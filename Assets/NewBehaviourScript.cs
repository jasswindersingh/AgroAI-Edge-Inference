using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Sentis; 

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Sentis AI Multi-Engine Matrix")]
    public ModelAsset soilModelAsset; 
    public ModelAsset diseaseModelAsset; 
    public ModelAsset cropModelAsset; 
    public ModelAsset treeModelAsset; 

    private Worker soilWorker;
    private Worker diseaseWorker; 
    private Worker cropWorker; 
    private Worker treeWorker; 

    private Texture2D mobileBufferTexture; 

    [Header("UI Canvas Screen Management")]
    public GameObject hubPanel;
    public GameObject soilProfilerPanel;
    public GameObject diseaseScannerPanel;
    public GameObject cropClassifierPanel;
    public GameObject treeIdentifierPanel;

    [Header("Soil Profiler Input Sliders")]
    public Slider nitrogenSlider;
    public Slider phosphorusSlider;
    public Slider potassiumSlider;
    public Slider temperatureSlider;
    public Slider humiditySlider;
    public Slider phSlider;
    public Slider rainfallSlider;

    [Header("Soil UI Typography Labels")]
    public TextMeshProUGUI soilDiagnosticOutputText;
    public TextMeshProUGUI nitrogenLabel;
    public TextMeshProUGUI phLabel;
    public TextMeshProUGUI rainfallLabel;

    [Header("Vision Readout UI Elements")]
    public TextMeshProUGUI diseaseOutputText; 
    public TextMeshProUGUI cropOutputText; 
    public TextMeshProUGUI treeOutputText; 
    public Image scanningReticle;

    [Header("Inference Thread Optimization")]
    public float inferenceInterval = 0.45f; 
    private int currentActiveMode = 0; // 0=Hub, 1=Soil, 2=Disease, 3=Crops, 4=Trees
    private Coroutine visionProcessingCoroutine;

    // --- CROP RECOMMENDATION MATRIX DATA MAPS (22 UNIQUE CLASSES IN OUTPUT) ---
    private string[] recommendedCropClasses = {
        "apple", "banana", "blackgram", "chickpea", "coconut", "coffee", "cotton", 
        "grapes", "jute", "kidneybeans", "lentil", "maize", "mango", "mothbeans", 
        "mungbean", "muskmelon", "orange", "papaya", "pigeonpeas", "pomegranate", "rice", "watermelon"
    };

    // --- INDIAN AGRO METRIC TRANSLATION MAPS ---
    private string[] diseaseClasses = { "diseased", "healthy" };
    private string[] cropClasses = { "cotton", "jute", "maize", "rice", "wheat" };
    
    // --- EXPANDED 48 ALPHABETICAL FLORA CLASSES FROM ONNX TRAINING DATASETS ---
    private string[] treeClasses = {
        "Aloevera", "Amla", "Amruthaballi", "Arali", "ashoka", "Astma_weed",
        "Badipala", "Balloon_Vine", "Bamboo", "Beans", "Betel", "Bhrami",
        "Caricature", "Castor", "Catharanthus", "Chakte", "Chilly", "Citron lime (herelikai)",
        "Common rue(naagdalli)", "Coriander", "Curry", "Doddpathre", "Drumstick", "Ekka",
        "Eucalyptus", "Gasagase", "Ginger", "Globe Amarnath", "Guava", "Henna",
        "Hibiscus", "Honge", "Insulin", "Jackfruit", "Jasmine", "Kambajala",
        "Kasambruga", "Lemon", "Malabar_Spinach", "Mango", "Marigold", "Mint",
        "Neem", "Nelavembu", "Rose", "Seethaashoka", "Tomato", "Turmeric"
    };

    void Start()
    {
        // Initializing all 4 standalone edge worker processors
        if (soilModelAsset != null) soilWorker = new Worker(ModelLoader.Load(soilModelAsset), BackendType.CPU);
        if (diseaseModelAsset != null) diseaseWorker = new Worker(ModelLoader.Load(diseaseModelAsset), BackendType.CPU);
        if (cropModelAsset != null) cropWorker = new Worker(ModelLoader.Load(cropModelAsset), BackendType.CPU);
        if (treeModelAsset != null) treeWorker = new Worker(ModelLoader.Load(treeModelAsset), BackendType.CPU);

        mobileBufferTexture = new Texture2D(224, 224, TextureFormat.RGB24, false);
        ShowHubWindow();
    }

    void Update()
    {
        if (currentActiveMode == 1 && soilWorker != null)
        {
            UpdateSoilSliderLabels();
            ExecuteSoilTabularInference();
        }

        if (currentActiveMode > 1 && scanningReticle != null)
        {
            float pulseScale = 1.0f + (Mathf.Sin(Time.time * 5.0f) * 0.05f);
            scanningReticle.transform.localScale = new Vector3(pulseScale, pulseScale, 1.0f);
        }
    }

    private void ExecuteSoilTabularInference()
    {
        if (soilDiagnosticOutputText == null) return;

        // Extracting all 7 parameters matching the structural format of your trained csv
        float[] features = new float[] {
            nitrogenSlider.value,
            phosphorusSlider.value,
            potassiumSlider.value,
            temperatureSlider.value,
            humiditySlider.value,
            phSlider.value,
            rainfallSlider.value
        };

        using Tensor<float> inputTensor = new Tensor<float>(new TensorShape(1, 7), features);
        soilWorker.Schedule(inputTensor);

        Tensor<float> outputTensor = soilWorker.PeekOutput() as Tensor<float>;
        float[] logits = outputTensor.DownloadToArray();

        int targetIdx = CalculateArgMax(logits);
        targetIdx = Mathf.Clamp(targetIdx, 0, recommendedCropClasses.Length - 1);

        // Algorithmic soil profile mapping derived from pH and chemical parameters
        string soilType = phSlider.value < 5.5f ? "Acidic Laterite Zone" : (phSlider.value > 7.5f ? "Alkaline Regur (Black) Zone" : "Loamy Alluvial Belt");
        float nitrogenRisk = nitrogenSlider.value < 40f ? 80f : 15f; 

        soilDiagnosticOutputText.text = $"<color=#00FFCC><b>[INDIAN CHEMICAL SOIL ENGINE ACTIVE]</b></color>\n\n" +
                                        $"<b>SOIL ZONE CLASSIFICATION:</b> <color=#FFD700>{soilType}</color>\n" +
                                        $"<b>ESTIMATED MOISTURE CAPILITY:</b> <color=#00FFCC>{humiditySlider.value:F1}% RH</color>\n" +
                                        $"<b>NITROGEN LEACHING RISK INDEX:</b> <color=#FF6666>{nitrogenRisk:F0}%</color>\n\n" +
                                        $"<b>💡 OPTIMAL TARGET SPECIFICATION:</b>\n" +
                                        $"This land composition matches conditions for growing: <color=#FFF><b>{recommendedCropClasses[targetIdx].ToUpper()}</b></color>.";
    }

    private void CaptureAndRunVisionInference()
    {
        int size = 224;
        int startX = (Screen.width - size) / 2; int startY = (Screen.height - size) / 2;
        if (startX < 0 || startY < 0) return;

        mobileBufferTexture.ReadPixels(new Rect(startX, startY, size, size), 0, 0);
        mobileBufferTexture.Apply();

        using Tensor<float> inputTensor = TextureConverter.ToTensor(mobileBufferTexture, size, size, 3);

        if (currentActiveMode == 2 && diseaseWorker != null) // Plant Health Mode
        {
            diseaseWorker.Schedule(inputTensor);
            float[] outputScores = ComputeSoftmax((diseaseWorker.PeekOutput() as Tensor<float>).DownloadToArray());
            int idx = CalculateArgMax(outputScores);
            idx = Mathf.Clamp(idx, 0, diseaseClasses.Length - 1);
            
            string remedy = idx == 0 ? "Apply target copper fungicides. Prune dead foliage." : "No intervention required. Maintain schedule.";
            diseaseOutputText.text = $"<b>HEALTH EVALUATION:</b> <color=#FFD700>{diseaseClasses[idx].ToUpper()}</color> ({outputScores[idx]*100:F0}%)\n\n" +
                                      $"<b>RECOMMENDED REMEDY:</b>\n<i>{remedy}</i>";
        }
        else if (currentActiveMode == 3 && cropWorker != null) // Field Crop Classification
        {
            cropWorker.Schedule(inputTensor);
            float[] outputScores = ComputeSoftmax((cropWorker.PeekOutput() as Tensor<float>).DownloadToArray());
            int idx = CalculateArgMax(outputScores);
            idx = Mathf.Clamp(idx, 0, cropClasses.Length - 1);
            
            cropOutputText.text = $"<b>IDENTIFIED FIELD CROP:</b>\n<color=#00FFCC><size=32>{cropClasses[idx].ToUpper()}</size></color>\n\n" +
                                   $"<b>CONFIDENCE RATING:</b> {outputScores[idx]*100:F1}%";
        }
        else if (currentActiveMode == 4 && treeWorker != null) // Tree Flora Recognition
        {
            treeWorker.Schedule(inputTensor);
            float[] outputScores = ComputeSoftmax((treeWorker.PeekOutput() as Tensor<float>).DownloadToArray());
            int idx = CalculateArgMax(outputScores);
            idx = Mathf.Clamp(idx, 0, treeClasses.Length - 1);
            
            // Clean up syntax characters from raw folder string syntax mapping
            string rawName = treeClasses[idx];
            string cleanName = rawName.Replace("_", " ");
            if (cleanName.Contains("(")) cleanName = cleanName.Split('(')[0].Trim();
            
            treeOutputText.text = $"<b>FLORA SPECIES:</b>\n<color=#FFD700><size=32>{cleanName.ToUpper()}</size></color>\n\n" +
                                   $"<b>ECOLOGICAL MATCH VALUE:</b> {outputScores[idx]*100:F1}%";
        }
    }

    private IEnumerator VisionProcessingLoop()
    {
        yield return new WaitForSeconds(0.3f);
        while (currentActiveMode > 1)
        {
            yield return new WaitForSeconds(inferenceInterval);
            yield return new WaitForEndOfFrame();
            CaptureAndRunVisionInference();
        }
    }

    public void ShowHubWindow() { currentActiveMode = 0; RouteUIPanels(true, false, false, false, false); StopVisionCoroutine(); }
    public void ShowSoilProfilerWindow() { currentActiveMode = 1; RouteUIPanels(false, true, false, false, false); StopVisionCoroutine(); }
    public void ShowDiseaseWindow() { currentActiveMode = 2; RouteUIPanels(false, false, true, false, false); LaunchVisionLoop("Analyzing crop foliage health..."); }
    public void ShowCropWindow() { currentActiveMode = 3; RouteUIPanels(false, false, false, true, false); LaunchVisionLoop("Targeting crop fields..."); }
    public void ShowTreeWindow() { currentActiveMode = 4; RouteUIPanels(false, false, false, false, true); LaunchVisionLoop("Analyzing canopy structures..."); }

    private void RouteUIPanels(bool hub, bool soil, bool disease, bool crop, bool tree)
    {
        hubPanel.SetActive(hub);
        soilProfilerPanel.SetActive(soil);
        diseaseScannerPanel.SetActive(disease);
        cropClassifierPanel.SetActive(crop);
        treeIdentifierPanel.SetActive(tree);
    }

    private void LaunchVisionLoop(string placeholderMsg)
    {
        StopVisionCoroutine();
        if (diseaseOutputText != null) diseaseOutputText.text = placeholderMsg;
        if (cropOutputText != null) cropOutputText.text = placeholderMsg;
        if (treeOutputText != null) treeOutputText.text = placeholderMsg;
        visionProcessingCoroutine = StartCoroutine(VisionProcessingLoop());
    }

    private void StopVisionCoroutine() { if (visionProcessingCoroutine != null) { StopCoroutine(visionProcessingCoroutine); visionProcessingCoroutine = null; } }

    private void UpdateSoilSliderLabels()
    {
        if (nitrogenLabel != null) nitrogenLabel.text = $"Nitrogen (N): <b>{nitrogenSlider.value:F0} mg/kg</b>";
        if (phLabel != null) phLabel.text = $"Soil Alkalinity: <b>{phSlider.value:F1} pH</b>";
        if (rainfallLabel != null) rainfallLabel.text = $"Annual Rainfall: <b>{rainfallSlider.value:F0} mm</b>";
    }

    private float[] ComputeSoftmax(float[] logits)
    {
        float[] results = new float[logits.Length];
        float maxVal = float.MinValue;
        for (int i = 0; i < logits.Length; i++) if (logits[i] > maxVal) maxVal = logits[i];
        float sum = 0.0f;
        for (int i = 0; i < logits.Length; i++) { results[i] = Mathf.Exp(logits[i] - maxVal); sum += results[i]; }
        for (int i = 0; i < logits.Length; i++) results[i] /= sum;
        return results;
    }

    private int CalculateArgMax(float[] array)
    {
        int index = 0; float maxVal = float.MinValue;
        for (int i = 0; i < array.Length; i++) if (array[i] > maxVal) { maxVal = array[i]; index = i; }
        return index;
    }

    public void ShutdownApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        StopVisionCoroutine();
        if (mobileBufferTexture != null) Destroy(mobileBufferTexture);
        if (soilWorker != null) soilWorker.Dispose();
        if (diseaseWorker != null) diseaseWorker.Dispose();
        if (cropWorker != null) cropWorker.Dispose();
        if (treeWorker != null) treeWorker.Dispose();
    }
}