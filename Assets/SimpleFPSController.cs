using UnityEngine;

public class SimpleFPSController : MonoBehaviour
{
    // VARIABLES PUBLIQUES (Visible et ajustable dans l'Inspector du Player)
    [Header("Camera Enfant")]
    // **CECI DOIT ÊTRE ASSIGNÉ DANS L'INSPECTOR** (glissez-déposez la Main Camera ici)
    public Transform cameraTransform; 
    
    [Header("Paramètres de Contrôle")]
    public float movementSpeed = 5.0f;
    public float mouseSensitivity = 2.0f;
    public float rotationLimit = 90.0f; // Limite le regard vertical à 90 degrés

    // VARIABLES PRIVÉES
    private float rotationX = 0; // Stocke la rotation verticale actuelle
    private CharacterController characterController;

    void Start()
    {
        // Récupère le CharacterController attaché à cet objet Player
        characterController = GetComponent<CharacterController>();
        
        // Sécurité : si cameraTransform n'est pas assigné, on essaie de la trouver
        if (cameraTransform == null)
        {
            Camera mainCam = GetComponentInChildren<Camera>();
            if (mainCam != null)
            {
                cameraTransform = mainCam.transform;
            }
            else
            {
                Debug.LogError("SimpleFPSController nécessite que la Main Camera soit son enfant et/ou assignée dans l'Inspector.");
            }
        }

        // Verrouille et masque le curseur de la souris (standard FPS)
        Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        // 1. Contrôle de la Tête (Regarder avec la Souris)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotation HORIZONTALE (Gauche/Droite) : Appliquée au Player (le corps)
        transform.Rotate(Vector3.up * mouseX);

        // Rotation VERTICALE (Haut/Bas) : Appliquée à la Camera enfant
        rotationX -= mouseY; // Inversé pour la convention standard de la souris
        rotationX = Mathf.Clamp(rotationX, -rotationLimit, rotationLimit);
        
        // Applique la rotation VERTICALE à la caméra enfant
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0); 
        
        
        // 2. Mouvement (WASD ou ZQSD)
        float moveForward = Input.GetAxis("Vertical") * movementSpeed;
        float moveSideways = Input.GetAxis("Horizontal") * movementSpeed;

        // Calcule le vecteur de mouvement
        Vector3 move = transform.forward * moveForward + transform.right * moveSideways;

        if (characterController != null)
        {
            // Applique la gravité
            if (!characterController.isGrounded)
            {
                // Ajout de la gravité (multiplié par Time.deltaTime pour la framerate)
                move.y += Physics.gravity.y * Time.deltaTime; 
            }
            
            // Déplace le CharacterController (collisions incluses)
            characterController.Move(move * Time.deltaTime);
        }
    }
}