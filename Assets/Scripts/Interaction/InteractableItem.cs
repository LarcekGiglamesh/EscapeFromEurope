using UnityEngine;
using System.Collections;

public class InteractableItem : MonoBehaviour
{
  [Range(10.0f, 50.0f)]
  public float m_mass = 10.0f;

  private Rigidbody rigidbody = null;

  private bool currentlyInteracting = false;

  private WandController attachedWand = null;

  private Transform interactionPoint = null;

  private float velocityFactor = 20000f;
  private float rotationFactor = 400f;
  private Vector3 posDelta = Vector3.zero;
  Quaternion rotationDelta = Quaternion.identity;
  float angle = 0.0f;
  Vector3 axis = Vector3.zero;


  void Start()
  {
    rigidbody = GetComponent<Rigidbody>();
    rigidbody.mass = m_mass;

    // this is why we have physics based interaction, the mass of the object is relevant for moving/rotating
    velocityFactor /= rigidbody.mass;
    rotationFactor /= rigidbody.mass;
	}
	
	void Update ()
  {
    if (attachedWand && currentlyInteracting)
    {
      // calculate movementDelta
      posDelta = attachedWand.transform.position - interactionPoint.position;
      rigidbody.velocity = posDelta * velocityFactor * Time.deltaTime;

      // calculate rotationDelta
      rotationDelta = attachedWand.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
      rotationDelta.ToAngleAxis(out angle, out axis);

      // could be deleted, just that the angle fits perfeclty...
      if (angle > 180)
      {
        angle -= 360;
      }

      // move the item
      this.rigidbody.angularVelocity = (Time.deltaTime * angle * axis) * rotationFactor;
    }
	}

  public void BeginInteraction(WandController wand)
  {
    attachedWand = wand;
    interactionPoint = new GameObject().transform;
    interactionPoint.gameObject.name = "[IP] " + this.gameObject.name;
    interactionPoint.position = wand.transform.position;
    interactionPoint.rotation = wand.transform.rotation;
    interactionPoint.SetParent(transform, true);

    //CollisionObserver observer = this.gameObject.AddComponent<CollisionObserver>();
    //observer.Init(wand);


    this.GetComponent<Renderer>().material.SetColor("_Color", Color.green);

    Physics.IgnoreCollision(GameObject.Find("[CameraRig]").GetComponent<Collider>(), this.GetComponent<Collider>(), true);

    currentlyInteracting = true;
  }

  public void EndInteraction(WandController wand)
  {
    if (wand == attachedWand)
    {
      attachedWand = null;
      currentlyInteracting = false;

      //Destroy(this.gameObject.GetComponent<CollisionObserver>());
      Physics.IgnoreCollision(GameObject.Find("[CameraRig]").GetComponent<Collider>(), this.GetComponent<Collider>(), false);
      this.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
      Destroy(interactionPoint.gameObject);
    }
  }

  public bool IsInteracting()
  {
    return currentlyInteracting;
  }

  public WandController GetWand()
  {
    return attachedWand;
  }
}
