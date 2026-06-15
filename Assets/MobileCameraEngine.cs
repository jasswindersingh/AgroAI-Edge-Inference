using UnityEngine;
using UnityEngine.UI;

public class MobileCameraEngine : MonoBehaviour
{
    private WebCamTexture backCameraDeviceStream;
    private RawImage displayTargetWindow;

    void Start()
    {
        displayTargetWindow = GetComponent<RawImage>();
        
        // Find the rear-facing camera on the Android device
        WebCamDevice[] availableDevices = WebCamTexture.devices;
        string rearCamName = "";

        for (int i = 0; i < availableDevices.Length; i++)
        {
            if (!availableDevices[i].isFrontFacing)
            {
                rearCamName = availableDevices[i].name;
                break;
            }
        }

        if (string.IsNullOrEmpty(rearCamName) && availableDevices.Length > 0)
        {
            rearCamName = availableDevices[0].name; // Fallback to primary if no back cam found
        }

        // Initialize camera texture stream optimized for edge AI processing dimensions
        backCameraDeviceStream = new WebCamTexture(rearCamName, 448, 448, 30);
        displayTargetWindow.texture = backCameraDeviceStream;

        if (!backCameraDeviceStream.isPlaying)
        {
            backCameraDeviceStream.Play();
        }
    }

    void OnEnable()
    {
        if (backCameraDeviceStream != null && !backCameraDeviceStream.isPlaying)
        {
            backCameraDeviceStream.Play();
        }
    }

    void OnDisable()
    {
        if (backCameraDeviceStream != null && backCameraDeviceStream.isPlaying)
        {
            backCameraDeviceStream.Stop();
        }
    }
}