using Cinemachine;
using DG.Tweening;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;


public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject cubePrefab;
    [SerializeField] GameObject cubeBelow;
    [SerializeField] List<GameObject> SpawnPointList;
    [SerializeField] GameObject background;
    [SerializeField] GameObject cameraFollowPoint;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] GameObject FirstCube;
    [SerializeField] TextMeshProUGUI TutorText;

    private GameObject overlappedPart;
    private GameObject cube;

    private int currentIndex;

    private int Score;
    private int HighestScore;
    public static bool GameOver = true;
    private bool gameStarted = false;

    private Vector3 topCubeScale;

     float Hue,V;
    private int Strike = 0;
    

    private void Start()
    {   
        HighestScore = PlayerPrefs.GetInt("HighestScore");
        UIManager.Instance.UpdateHighestScore(HighestScore);
    }

    private void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.Space) && GameOver == false  && !UIManager.Instance.OpenMenu) {


            if (TutorText.gameObject.activeInHierarchy && gameStarted == true )
{
             TutorText.gameObject.SetActive(false);
}

            if (currentIndex == 0) {
                currentIndex = 1;
            } else { currentIndex = 0; }

            

            DOTween.KillAll();

            if (cube != null)
            {               
               cube.GetComponent<Rigidbody>().useGravity = true;
               SplitCube(cube, cubeBelow);     
                
                transform.position = transform.position + new Vector3(0, 0.25f, 0);
                background.transform.position += new Vector3(0, 0.25f, 0);
                cameraFollowPoint.transform.position += new Vector3(0, 0.25f, 0);
            }
          
            

            if ( !GameOver)
            {
                GameObject nextCube = SpawnCube(cubePrefab, currentIndex,topCubeScale);
                CubeMove(nextCube, currentIndex);
                cube = nextCube;
                
                V += 6;
                Hue += 3;
                ChangeColor(nextCube, Hue, V);
                // ChangeBackgroundColor(H);
            }

        }
    }

    public void SplitCube(GameObject cAbove, GameObject cBelow)
    {   

        // Lấy kích thước và vị trí của hai Cube
        Bounds aboveBounds = cAbove.GetComponent<Renderer>().bounds;
        Bounds belowBounds = cBelow.GetComponent<Renderer>().bounds;

        
        if (!aboveBounds.Intersects(belowBounds))
        {
            Debug.LogWarning("Game Over");
            ZoomoutCamera();
            GameOver = true;
            UIManager.Instance.reStartBtn.gameObject.SetActive(true);
            return;
        }

        Score += 1;

        UIManager.Instance.UpdateScore(Score);
        if (Score > PlayerPrefs.GetInt("HighestScore")){ 
            PlayerPrefs.SetInt("HighestScore", Score);
            HighestScore = Score;
        }
        UIManager.Instance.UpdateHighestScore(HighestScore);



       
       
        float overlapMinX = Mathf.Max(aboveBounds.min.x, belowBounds.min.x);
        float overlapMaxX = Mathf.Min(aboveBounds.max.x, belowBounds.max.x);

        float overlapMinZ = Mathf.Max(aboveBounds.min.z, belowBounds.min.z);
        float overlapMaxZ = Mathf.Min(aboveBounds.max.z, belowBounds.max.z);

        float overlapWidth = overlapMaxX - overlapMinX;
        float overlapDepth = overlapMaxZ - overlapMinZ;

        if (overlapWidth <= 0f || overlapDepth <= 0f) return;

        float deltaX = Mathf.Abs(cAbove.transform.position.x - cBelow.transform.position.x);
        float deltaZ = Mathf.Abs(cAbove.transform.position.z - cBelow.transform.position.z);


        if ( deltaX <= 0.1f && deltaZ <= 0.1f)
        {
            Strike += 1;
            Debug.Log("Strike"+Strike);
            overlappedPart = CubePooling.instance.GetCubeFromPool();
            overlappedPart.transform.localScale = new Vector3(
                cAbove.transform.localScale.x,
                cAbove.transform.localScale.y,
                cAbove.transform.localScale.z
            );
            overlappedPart.transform.position = new Vector3(
                (cBelow.transform.position.x),
                cAbove.transform.position.y,
                cBelow.transform.position.z 
            );


            overlappedPart.GetComponent<Rigidbody>().isKinematic = true;
            topCubeScale = overlappedPart.transform.localScale;
            ChangeColor(overlappedPart, Hue, V);
          
            transform.position = new Vector3(overlappedPart.transform.position.x, transform.position.y, overlappedPart.transform.position.z);


            if (Strike % 6 == 5 )
            {
                float newXScale = Mathf.Clamp(overlappedPart.transform.localScale.x + 0.08f,0f, 1f);
                float newZScale = Mathf.Clamp(overlappedPart.transform.localScale.z + 0.08f, 0f, 1f);
                Debug.Log("Increase");
                overlappedPart.transform.transform.localScale = new Vector3(newXScale, 0.25f, newZScale);                
                topCubeScale = new Vector3(newXScale, 0.25f,newZScale);
            }

            cubeBelow = overlappedPart;

            CubePooling.instance.ReturnCubeToPool(cAbove);

           

            return;
        }

        Strike = 0;
        Debug.Log("Strike" + Strike);


       

        // Tạo phần cube còn lại ( phần nằm trên khối cũ ) 

        overlappedPart =  CubePooling.instance.GetCubeFromPool();
        overlappedPart.transform.localScale = new Vector3(
            overlapWidth,
            cAbove.transform.localScale.y,
            overlapDepth
        );
        overlappedPart.transform.position = new Vector3(
            (overlapMinX + overlapMaxX) / 2,
            cAbove.transform.position.y,
            (overlapMinZ + overlapMaxZ) / 2
        );


        overlappedPart.GetComponent<Rigidbody>().isKinematic = true;
        topCubeScale =  overlappedPart.transform.localScale;
        ChangeColor(overlappedPart,Hue,V);

        

        transform.position = new Vector3(overlappedPart.transform.position.x,transform.position.y, overlappedPart.transform.position.z);
        cubeBelow = overlappedPart;

        
        // Tính toán phần thừa ra (bên trái, phải, trước và sau)
        float leftCutWidth = aboveBounds.min.x < overlapMinX ? overlapMinX - aboveBounds.min.x : 0;
        float rightCutWidth = aboveBounds.max.x > overlapMaxX ? aboveBounds.max.x - overlapMaxX : 0;
        float frontCutDepth = aboveBounds.min.z < overlapMinZ ? overlapMinZ - aboveBounds.min.z : 0;
        float backCutDepth = aboveBounds.max.z > overlapMaxZ ? aboveBounds.max.z - overlapMaxZ : 0;

     

        // Tạo phần thừa bên trái
        if (leftCutWidth > 0)
        {
            GameObject leftPart = CubePooling.instance.GetCubeFromPool();
            leftPart.GetComponent<Rigidbody>().useGravity = true;
            leftPart.transform.localScale = new Vector3(
                leftCutWidth,
                cAbove.transform.localScale.y,
                cAbove.transform.localScale.z
            );
            leftPart.transform.position = new Vector3(
                aboveBounds.min.x + leftCutWidth / 2 - 0.05f,
                cAbove.transform.position.y,
                cAbove.transform.position.z
            );
            ChangeColor(leftPart, Hue, V);
        }

        // Tạo phần thừa bên phải
        if (rightCutWidth > 0)
        {
            GameObject rightPart = CubePooling.instance.GetCubeFromPool();
            rightPart.GetComponent<Rigidbody>().useGravity = true;
            rightPart.transform.localScale = new Vector3(
                rightCutWidth,
                cAbove.transform.localScale.y,
                cAbove.transform.localScale.z
            );
            rightPart.transform.position = new Vector3(
                aboveBounds.max.x - rightCutWidth / 2 + 0.05f,
                cAbove.transform.position.y,
                cAbove.transform.position.z
            );
            ChangeColor(rightPart, Hue, V);
        }

        // Tạo phần thừa phía trước
        if (frontCutDepth > 0)
        {
            GameObject frontPart = CubePooling.instance.GetCubeFromPool();
            frontPart.GetComponent<Rigidbody>().useGravity = true;
            frontPart.transform.localScale = new Vector3(
                cAbove.transform.localScale.x,
                cAbove.transform.localScale.y,
                frontCutDepth
            );
            frontPart.transform.position = new Vector3(
                cAbove.transform.position.x,
                cAbove.transform.position.y,
                aboveBounds.min.z + frontCutDepth / 2 - 0.05f
            );
            ChangeColor(frontPart, Hue, V);
        }

        // Tạo phần thừa phía sau
        if (backCutDepth > 0)
        {
            GameObject backPart = CubePooling.instance.GetCubeFromPool();
            backPart.GetComponent<Rigidbody>().useGravity = true;
            backPart.transform.localScale = new Vector3(
                cAbove.transform.localScale.x,
                cAbove.transform.localScale.y,
                backCutDepth
            );
            backPart.transform.position = new Vector3(
                cAbove.transform.position.x,
                cAbove.transform.position.y,
                aboveBounds.max.z - backCutDepth / 2 +0.05f
            );
            ChangeColor(backPart, Hue, V);
        }

        // Xóa Cube ban đầu
        CubePooling.instance.ReturnCubeToPool(cAbove);
    }


    public void StartGame()
    { 
        
        if(gameStarted == false)
        {
            gameStarted = true;
            GameOver = false;
        }
        



        Hue = Random.Range(0, 361);
        V = 10;

        Score = 0;
        UIManager.Instance.UpdateScore(Score);
        currentIndex = Random.Range(0, 2);
        cube = SpawnCube(cubePrefab, currentIndex, new Vector3(1, 0.25f, 1));
        ChangeColor(cube, Hue, V);
        ChangeColor(FirstCube, Hue, 0);
        CubeMove(cube, currentIndex);
    }

        
    public GameObject SpawnCube(GameObject cube,int index, Vector3 scale)
    {
        if (SpawnPointList == null || SpawnPointList.Count == 0)
        {
            Debug.LogError("SpawnPointList is empty or not assigned!");
            return null;
        }

       
   
        Vector3 spawnPosition = new Vector3(
            SpawnPointList[index].transform.position.x,
            gameObject.transform.position.y,
            SpawnPointList[index].transform.position.z
        );

        Quaternion spawnRotation = Quaternion.identity;
        GameObject obj = CubePooling.instance.GetCubeFromPool();
         obj .transform.rotation = spawnRotation;
         obj.transform.position = spawnPosition;
         obj.transform.localScale = scale;

        return obj;
    }

    public void CubeMove(GameObject cube, int index)
    {
        Sequence cubeSequence = DOTween.Sequence(); // Tạo chuỗi di chuyển

        switch (index)
        {
           
            //2
            case 0:
                cube.transform.DOMoveX(-1.2f, 1f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    // Bắt đầu Sequence sau khi tween đầu tiên hoàn tất
                    Sequence cubeSequence = DOTween.Sequence();
                    cubeSequence.Append(cube.transform.DOMoveX(1.2f, 1f).SetEase(Ease.InOutSine))
                                .Append(cube.transform.DOMoveX(-1.2f, 1f).SetEase(Ease.InOutSine))
                                .SetLoops(-1, LoopType.Restart);
                });
                break;

          

            case 1: // Trục Z, bắt đầu từ 2
                cube.transform.DOMoveZ(-1.2f, 1f).SetEase(Ease.InOutSine).OnComplete(() =>
                {                  
                    Sequence cubeSequence = DOTween.Sequence();
                    cubeSequence.Append(cube.transform.DOMoveZ(1.2f, 1f).SetEase(Ease.InOutSine))
                                .Append(cube.transform.DOMoveZ(-1.2f, 1f).SetEase(Ease.InOutSine))
                                .SetLoops(-1, LoopType.Restart);
                });
                break;

            default:
                Debug.LogWarning("Invalid index provided.");
                break;
        }
    }


    public void ZoomoutCamera()
    {
        if (virtualCamera != null)
        {
           
            DOTween.To(() => virtualCamera.m_Lens.OrthographicSize,
                       x => virtualCamera.m_Lens.OrthographicSize = x,
                       4.3f, 1f).SetEase(Ease.Linear);

            background.transform.DOScale(new Vector3(2f, 1f, 2.5f), 1f).SetEase(Ease.Linear);
        }
    }

    public void ZoominCamera()
    {
        if (virtualCamera != null)
        {

            DOTween.To(() => virtualCamera.m_Lens.OrthographicSize,
                       x => virtualCamera.m_Lens.OrthographicSize = x,
                       1.8f, 1f).SetEase(Ease.Linear);
            background.transform.DOScale(new Vector3(2f, 1f, 1.25f), 1f)
              .SetEase(Ease.Linear)
              .OnComplete(() =>
              {  
                  TutorText.gameObject.SetActive(true);
                  GameOver = false; 
              });
        }
    }

    public void ChangeColor(GameObject obj, float H, float V)
    {  
       

        H = (H % 360)/360;
        V = (V % 60 + 40)/100;

        Renderer  objRenderer = obj.GetComponent<Renderer>();
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_BaseColor", UnityEngine.Color.HSVToRGB(H, 0.55f, V));
        objRenderer.SetPropertyBlock(propertyBlock);
     

    }

    public void ChangeBackgroundColor(float H)
    {
        
        
        
        Renderer backgroundRenderer = background.GetComponent<Renderer>();
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_TopColor", UnityEngine.Color.HSVToRGB(((H + 180) % 360)/360, 0.5f, 1));
        propertyBlock.SetColor("_BottomColor", UnityEngine.Color.HSVToRGB(((H ) % 360) / 360, 0.5f, 1));
        backgroundRenderer.SetPropertyBlock(propertyBlock);
    }

    public void RestartGame()
    {   
        
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject cube in cubes)
        {
            cube.GetComponent<Cube>().ReturnToPool();
        }

        background.transform.position = new Vector3(background.transform.position.x, -8f, background.transform.position.z);
        transform.position = new Vector3(0f,0.625f,0f);
        cameraFollowPoint.transform.position = new Vector3(0f, 0.625f, 0f);
        ZoominCamera();
        cubeBelow = FirstCube;
        StartGame();
    }


}



