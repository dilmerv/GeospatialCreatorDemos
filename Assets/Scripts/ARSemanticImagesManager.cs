using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.UI;

public class ARSemanticImagesManager : MonoBehaviour
{
    [SerializeField]
    private RawImage semanticRawImage;

    private Texture2D semanticTexture2D;

    private ARSemanticManager semanticManager;

    private bool semanticTextureObtained;

    private bool isSemanticModeAvailable;

    private void Awake()
    {
        semanticManager = FindObjectOfType<ARSemanticManager>();

        // check for support
        if(semanticManager.IsSemanticModeSupported(SemanticMode.Enabled) == FeatureSupported.Supported)
        {
            Logger.Instance.LogWarning("Semantic Segmentation is supported on this device");
            isSemanticModeAvailable = true;
        }
        else
        {
            Logger.Instance.LogWarning("Unfortunately Semantic Segmentation is not supported on this device");    
        }
    }

    void Update()
    {
        if (!isSemanticModeAvailable) return;

        if (semanticManager.TryGetSemanticTexture(ref semanticTexture2D))
        {
            semanticRawImage.texture = semanticTexture2D;
            semanticTextureObtained = true;
        }
        else
        {
            semanticTextureObtained = false;
        }

        Logger.Instance.LogInfo($"Person probability in view: {semanticManager.GetSemanticLabelFraction(SemanticLabel.Person)}");
    }
}
