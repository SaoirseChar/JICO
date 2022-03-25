using System.Collections;
using UnityEngine;
using UnityEngine.AI;


//Pet Behaviours
public enum PetAI
{
    walk,
    feed
}

public class StateMachine : MonoBehaviour
{
    [Header("Walk State Stats")]
    private NavMeshAgent nav;
    private Animator anim;
    private int destination = 0;
    [SerializeField] private Transform fetchPoint;
    public Transform startPoint;
    [SerializeField] private float dropBallRadius;
    [SerializeField] private BallPosition ballPos;

    //Pet States
    public PetAI state;

    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        nav.autoBraking = false; //No breaking at all

        StartCoroutine(WalkState());
    }

    private void FixedUpdate()
    {
        //Attempting to drop the ball on the ground when pet reaches the end point
        if (Vector3.Distance(nav.transform.position, startPoint.gameObject.transform.position) < dropBallRadius)
        {
            ballPos.RemovePet();
            ballPos = null;
        }
    }

    #region IEnumerator States
    private IEnumerator WalkState()
    {
        while (state == PetAI.walk)
        {
            yield return null;

            if (_GameManager.instance != null)
            {
                if (_GameManager.instance.clickCount == 0) //If not being clicked on, move around the room
                {
                    //Walk animation
                    anim.SetInteger("Walk", 1);
                    Walk();
                }
                else
                {
                    //Stop walking
                    anim.SetInteger("Walk", 0);
                }
            }
        }

        state = PetAI.feed;
        StartCoroutine(FeedState());
    }

    private IEnumerator FeedState()
    {
        while (state == PetAI.feed)
        {
            //Feeding Pet Code Here
            //Make pet move to start position, then put in feed state
            nav.destination = Vector3.MoveTowards(transform.position, _GameManager.instance.walkPoints[3].transform.position, Time.deltaTime * nav.speed);
            _GameManager.instance.FeedPet();
            yield return null;
        }
        state = PetAI.walk;
        StartCoroutine(WalkState());
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Pet picking up the ball
        if (collision.gameObject.TryGetComponent<BallPosition>(out BallPosition obj))
        {
            obj.gameObject.transform.SetParent(fetchPoint);
            obj.SetPet();
            ballPos = obj;
        }
    }
    #endregion

    private IEnumerator DropBall()
    {
        yield return new WaitForSeconds(1f);
    }

    #region Functions
    public void Walk()
    {
        Debug.Log("Pet is moving to next point");
        //If no more points to travel to, return
        if (_GameManager.instance != null)
        {
            if (Vector3.Distance(_GameManager.instance.walkPoints[destination].transform.position, transform.position) < 0.5f)
            {
                destination++;

                if (destination >= _GameManager.instance.walkPoints.Length)
                {
                    destination = 0;
                }
            }
            nav.destination = Vector3.MoveTowards(transform.position, _GameManager.instance.walkPoints[destination].transform.position, Time.deltaTime * nav.speed);
        }
    }
    #endregion

    //Function for calling the next state for pets
    private void NextState()
    {
        //Work out the name of the method to run
        string methodName = state.ToString() + "State"; //If current state is "walk" then this returns "walkState"
                                                        //gives a variable so run a method using its name
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);
        //Run our method
        StartCoroutine((IEnumerator)info.Invoke(this, null));
        //Using StartCoroutine() means we can leave and come back to the method that is running
        //All Coroutines must return IEnumerator
    }
}
