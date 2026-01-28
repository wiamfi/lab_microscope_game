using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Needed for the IEnumerator!
using TMPro;
public class MicroscopeStation : MonoBehaviour
{
    [Header("Player Reference")]
    public SimpleFPSController playerController; 

    [Header("UI References")]
    public GameObject microscopeUI; 
    public RawImage displayImage;   
    public RawImage blurOverlay; // The duplicate you just made

    [Header("Zoom Textures")]
    public Texture[] zoomTextures;  
    
    private int currentZoom = 0;
    private bool isUsing = false;
    public TextMeshProUGUI zoomText; // Drag ZoomLabel here
    public string[] zoomLevels;      // e.g., "40x", "100x", "400x"
    void Update()
    {
        float dist = Vector3.Distance(transform.position, Camera.main.transform.position);

        if (dist < 2.5f && Input.GetKeyDown(KeyCode.E))
        {
            ToggleMicroscope();
        }

        if (isUsing)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) 
            {
                currentZoom = (currentZoom + 1) % zoomTextures.Length;
                UpdateDisplay();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                currentZoom--;
                if (currentZoom < 0) currentZoom = zoomTextures.Length - 1;
                UpdateDisplay();
            }
        }
    }

    void ToggleMicroscope()
    {
        isUsing = !isUsing;
        microscopeUI.SetActive(isUsing);
        
        if (playerController != null) playerController.enabled = !isUsing;

        if (isUsing)
        {
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true; 
            UpdateDisplay();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; 
            Cursor.visible = false;
        }
    }

    void UpdateDisplay()
    {
        if (zoomTextures.Length > 0)
        {
            displayImage.texture = zoomTextures[currentZoom];
            blurOverlay.texture = zoomTextures[currentZoom];

            // Update the text label
            if (zoomLevels.Length > currentZoom)
            {
                zoomText.text = zoomLevels[currentZoom];
            }

            StopAllCoroutines();
            StartCoroutine(LensSwapEffect());
        }
    }

    // THE UPDATED FLICKER CODE
    IEnumerator LensSwapEffect()
    {
        // 1. Move the blur to a random "shaky" spot
        float randomX = Random.Range(-10f, 10f);
        float randomY = Random.Range(-10f, 10f);
        blurOverlay.rectTransform.anchoredPosition = new Vector2(randomX, randomY);

        // 2. Turn the blur on and pop the scale
        blurOverlay.enabled = true;
        displayImage.transform.localScale = new Vector3(1.05f, 1.05f, 1f);

        // 3. Wait a tiny bit (realistic lens rotation speed)
        yield return new WaitForSeconds(0.12f);

        // 4. Turn everything back to normal
        blurOverlay.enabled = false;
        displayImage.transform.localScale = Vector3.one;
    }
}