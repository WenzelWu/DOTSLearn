using UnityEngine;

namespace LearnJobs
{
    public class Spawner : MonoBehaviour
    {
        public static Transform[] TargetTransform;
        public static Transform[] SeekerTransform;
        
        public GameObject SeekerPrefab;
        public GameObject TargetPrefab;

        public int NumSeekers;
        public int NumTargets;
        
        public Vector2 Bounds;

        public void Start()
        {
            Random.InitState(123);
        
            SeekerTransform = new Transform[NumSeekers];
            for (int i = 0; i < NumSeekers; i++)
            {
                GameObject go = Instantiate(SeekerPrefab);
                Seeker seeker = go.GetComponent<Seeker>();
                Vector2 dir = Random.insideUnitCircle;
                seeker.Direction = new Vector3(dir.x, 0, dir.y);
                go.transform.localPosition = new Vector3(
                    Random.Range(0, Bounds.x), 0, Random.Range(0, Bounds.y));
                SeekerTransform[i] = go.transform;
            }

            TargetTransform = new Transform[NumTargets];

            for (int i = 0; i < NumTargets; i++)
            {
                GameObject go = Instantiate(TargetPrefab);
                Target target = go.GetComponent<Target>();
                Vector2 dir = Random.insideUnitCircle;
                target.Direction = new Vector3(dir.x, 0, dir.y);
                go.transform.localPosition = new Vector3(
                    Random.Range(0, Bounds.x), 0, Random.Range(0, Bounds.y));
                TargetTransform[i] = go.transform;
            }
        }
    }
}