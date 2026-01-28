using UnityEngine;

public class ObjectGrabber : MonoBehaviour
{ 
    public Transform cameraTransform;
    public float grabDistance = 3f; // Distance max pour saisir un objet
    public float carryDistance = 2f; // Distance de l'objet lorsqu'il est tenu
    
    private Rigidbody heldObject = null;

    void Update()
    {
        // Si le clic gauche est pressé
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null)
            {
                // Tenter de saisir un objet
                TryGrabObject();
            }
            else
            {
                // Lâcher l'objet
                ReleaseObject();
            }
        }

        // Déplacer l'objet tenu
        if (heldObject != null)
        {
            Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * carryDistance;
            // Déplacer physiquement l'objet vers la cible
            heldObject.position = targetPosition;
            // Empêcher l'objet de tourner
            heldObject.rotation = Quaternion.identity; 
        }
    }

    void TryGrabObject()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;
        // On vérifie si le rayon touche un objet dans la plage 'grabDistance'
        // Radius 0.2f makes the "laser" as thick as a tennis ball
if (Physics.SphereCast(ray, 0.2f, out hit, grabDistance))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                heldObject = rb;
                // Désactive temporairement la gravité et le rend Kinematic (contrôlé par le script)
                heldObject.useGravity = false;
                heldObject.isKinematic = true; 
            }
        }
    }

    void ReleaseObject()
    {
        if (heldObject != null)
        {
            // Rétablit la physique
            heldObject.isKinematic = false;
            heldObject.useGravity = true;
            heldObject.linearVelocity = Vector3.zero;
            heldObject = null;
        }
    }
}