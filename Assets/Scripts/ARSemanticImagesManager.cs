using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ARSemanticImagesManager : MonoBehaviour
{
    private static readonly int ColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int TextureId = Shader.PropertyToID("_BaseMap");

    [SerializeField]
    private Camera semanticCamera;

    [SerializeField]
    private GameObject semanticsLayerQuad;

    [SerializeField]
    private GameObject semanticInfoPrefab;

    [SerializeField]
    private Slider semanticAlpha;

    [SerializeField]
    private Toggle showSemanticConfidenceImage;

    [SerializeField]
    private RawImage semanticConfidenceImage;

    [SerializeField]
    private GameObject semanticInfoLayout;

    [SerializeField]
    private float semanticImageInterval = 0.25f;

    [SerializeField]
    private float semanticInfoInterval = 0.50f;

    [SerializeField]
    private LabelMapping[] mappings;

    [SerializeField]
    private float checkForSupportFrequency = 0.1f;

    [SerializeField]
    private int maxAttempts = 5;

    private ARSemanticManager semanticManager;

    private bool isSemanticModeAvailable;

    private Dictionary<SemanticLabel, GameObject> semanticInfos = new Dictionary<SemanticLabel, GameObject>();

    private Texture2D inputTexture;

    private Texture2D inputConfidenceTexture;

    private Texture2D outputTexture;

    private Renderer semanticsLayerQuadRenderer;

    private GameObject semanticConfidenceContainer;


    private void Awake()
    {
        semanticManager = GetComponent<ARSemanticManager>();
        semanticConfidenceContainer = semanticConfidenceImage.transform.parent.gameObject;

        semanticsLayerQuad.transform.localScale = new Vector3(semanticCamera.orthographicSize * 2.0f * Screen.width / Screen.height,
            semanticCamera.orthographicSize * 2.0f, 0.1f);

        semanticsLayerQuadRenderer = semanticsLayerQuad.GetComponent<Renderer>();

        showSemanticConfidenceImage.onValueChanged.AddListener(isOn =>
        {
            semanticConfidenceContainer.SetActive(isOn);
        });

        semanticConfidenceContainer.SetActive(showSemanticConfidenceImage.isOn);
    }

    private void Start()
    {
        //StartCoroutine(CheckForSemanticFeatureSupport());
        //StartCoroutine(ProcessSemanticsImage());
        //StartCoroutine(UpdateSemanticInfo());
    }

    private IEnumerator CheckForSemanticFeatureSupport()
    {
        //TODO - to be implemented
        yield return null;
    }

    private IEnumerator UpdateSemanticInfo()
    {
        //TODO - to be implemented
        yield return null;
    }

    private IEnumerator ProcessSemanticsImage()
    {
        //TODO - to be implemented
        yield return null;
    }

    private bool RequestSemanticTexture(ref Texture2D result)
    {
        //TODO - to be implemented
        result = new Texture2D(0,0);
        return true;
    }


    private Color GetColor(SemanticLabel label)
    {
        if (label == SemanticLabel.Unlabeled) return Color.gray;

        foreach (var mapping in mappings)
        {
            if (mapping.label == label)
            {
                return mapping.color;
            }
        }

        return Color.black;
    }

    private void ConvertR8ToRGBA32Flipped(ref Texture2D inputTexture, ref Texture2D rgbaTexture)
    {
        int width = inputTexture.width;
        int height = inputTexture.height;

        rgbaTexture = new Texture2D(height, width, TextureFormat.RGBA32, false);

        var rawTextureData = inputTexture.GetRawTextureData<byte>();
        var pixels = new Color[rawTextureData.Length];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var label = (SemanticLabel)rawTextureData[i * width + j];
                Color color = GetColor(label);
                color.a = semanticAlpha.value;
                int index = (rgbaTexture.width * rgbaTexture.height) - (j * rgbaTexture.width + i + 1);
                pixels[index] = color;
            }
        }

        rgbaTexture.SetPixels(pixels);
        rgbaTexture.Apply();
    }
}
