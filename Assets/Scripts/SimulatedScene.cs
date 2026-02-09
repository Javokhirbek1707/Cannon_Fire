using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulatedScene : MonoBehaviour
{
    private Scene _scene;
    private PhysicsScene _physicsScene;

    [Header("Trajectory")]
    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _steps = 60;
    [SerializeField] private float _timeStep = 0.05f;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _groundBounceLoss = 0.6f; 
    private ShotScript _ghost;
    private Rigidbody _ghostRB;
    private Vector3[] _points;

    private void Awake()
    {
        _scene = SceneManager.CreateScene("SimulatedPhysicsScene",new CreateSceneParameters(LocalPhysicsMode.Physics3D));

        _physicsScene = _scene.GetPhysicsScene();

        if (_line == null)
            _line = GetComponent<LineRenderer>();

        BuildEnvironment();
    }

    private void BuildEnvironment()
    {
        foreach (GameObject root in _scene.GetRootGameObjects())
            Destroy(root);

        // Clone obstacles
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Obstacle"))
            CloneIntoSimScene(go);

        int groundLayer = LayerMask.NameToLayer("Ground");
        var all = FindObjectsOfType<Transform>(true);
        foreach (var t in all)
        {
            if (t.gameObject.layer != groundLayer) continue;

            // clone only top-most ground objects
            if (t.parent != null && t.parent.gameObject.layer == groundLayer) continue;

            CloneIntoSimScene(t.gameObject);
        }
    }

    private void CloneIntoSimScene(GameObject go)
    {
        GameObject clone = Instantiate(go, go.transform.position, go.transform.rotation);

        foreach (Renderer r in clone.GetComponentsInChildren<Renderer>())
            r.enabled = false;

        foreach (MonoBehaviour m in clone.GetComponentsInChildren<MonoBehaviour>())
            m.enabled = false;

        foreach (Rigidbody rb in clone.GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = true;

        SceneManager.MoveGameObjectToScene(clone, _scene);
    }

    public void SimulateTrajectory(ShotScript prefab, Vector3 startPos, Vector3 force)
    {
        if (!_physicsScene.IsValid()) return;
        if (_line == null) return;
        if (prefab == null) return;

        if (_ghost == null)
        {
            _ghost = Instantiate(prefab, startPos, Quaternion.identity);

            foreach (Renderer r in _ghost.GetComponentsInChildren<Renderer>())
                r.enabled = false;

            SceneManager.MoveGameObjectToScene(_ghost.gameObject, _scene);
            _ghostRB = _ghost.GetComponent<Rigidbody>();
        }

        if (_points == null || _points.Length != _steps)
            _points = new Vector3[_steps];

        // Reset ghost
        _ghost.transform.position = startPos;
        _ghost.transform.rotation = Quaternion.identity;
        _ghostRB.velocity = Vector3.zero;
        _ghostRB.angularVelocity = Vector3.zero;
        _ghostRB.Sleep();
        _ghostRB.WakeUp();

        _ghostRB.AddForce(force, ForceMode.Impulse);

        _points[0] = startPos;
        int pointCount = 1;

        Vector3 velocity = _ghostRB.velocity;

        for (int i = 1; i < _steps; i++)
        {
            Vector3 prevPos = _ghostRB.position;

            _physicsScene.Simulate(_timeStep);

            Vector3 currPos = _ghostRB.position;
            _points[i] = currPos;
            pointCount++;

            if (RaycastGround(prevPos, currPos, out RaycastHit hit))
            {
                _ghostRB.position = hit.point;

                // reflect velocity using ground normal
                velocity = Vector3.Reflect(velocity, hit.normal);
                velocity *= _groundBounceLoss;

                _ghostRB.velocity = velocity;
            }
            else
            {
                velocity = _ghostRB.velocity;
            }
        }

        _line.positionCount = pointCount;
        _line.SetPositions(_points);
    }

    private bool RaycastGround(Vector3 from, Vector3 to, out RaycastHit hit)
    {
        Vector3 dir = to - from;
        float dist = dir.magnitude;

        hit = default;

        if (dist < 0.0001f)
            return false;

        return _physicsScene.Raycast(from,dir.normalized,out hit,dist,_groundMask);
    }
}