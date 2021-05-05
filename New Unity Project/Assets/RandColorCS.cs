using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandColorCS : MonoBehaviour
{

    float velInicial, velFinal;
    float dist, posFinal, posicaoinicial, gravidade, tempofinal, dT;
    float movendo;

// V = VelocidadeInicial + aceleração x tempo;
    struct Cube 
    {
        public Vector3 position;
        public Color color;
        public float dT, velFinal, velInicial, gravidade, posFinal, dist;
        
    }

    public ComputeShader computeShader;
    public int iteractions = 50;
    public int count = 100;
    GameObject[] gameObjects;
    Cube[] data;

    public GameObject modelPref;

    bool btmCreatepressed = false;
    bool btmGPUpressed = false;

    bool btmCPUpressed = false;

    public double tempo, tempoinicio;
    public GameObject floorModel;
    // Start is called before the first frame update
    void Start()
    {
        floorModel = GameObject.Instantiate(floorModel, new Vector3(0,-201,0), Quaternion.identity);
        floorModel.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);

        velInicial = 0;
        
        gravidade = 9.8f;
    }

    // Update is called once per frame
    void Update()
    {
       
        
        if(btmCPUpressed){
        

        dT += Time.deltaTime;

        velFinal = velInicial + gravidade * dT;

        dist = ((velFinal + velInicial)) * dT;

        posFinal = posFinal - dist;
            
        for(int i = 0; i < gameObjects.Length; i++){

        if(gameObjects[i].transform.position.y > -200f){  
            gameObjects[i].transform.position = new Vector3( gameObjects[i].transform.position.x, posFinal, 
            gameObjects[i].transform.position.z);
        
            }else{
            gameObjects[i].transform.position = new Vector3( gameObjects[i].transform.position.x, -200.75f, 
            gameObjects[i].transform.position.z);
            
        
            tempo = Time.realtimeSinceStartup - tempoinicio;
            Debug.Log("O tempo em CPU foi " + tempo);
            btmCPUpressed = false;
            }
        }
        }

        if(btmGPUpressed){
    
            int totalSize = 4 * sizeof(float) + 3 * sizeof(float) + 6* sizeof(float);

            ComputeBuffer computeBuffer = new ComputeBuffer(data.Length, totalSize);
            computeBuffer.SetData(data);

            computeShader.SetBuffer(0, "cubes", computeBuffer);
            computeShader.SetInt("iteraction", iteractions);
            computeShader.SetFloat("time", Time.deltaTime);

            computeShader.Dispatch(0, data.Length / 10, 1, 1);

            computeBuffer.GetData(data);

            for (int i = 0; i < gameObjects.Length; i++)
                {
                    if(gameObjects[i].transform.position.y > -200f){
                        gameObjects[i].transform.position = new Vector3( gameObjects[i].transform.position.x, 
                        data[i].posFinal, gameObjects[i].transform.position.z);
                    }else{
                        gameObjects[i].transform.position = new Vector3( gameObjects[i].transform.position.x, -200.75f, 
                        gameObjects[i].transform.position.z); 
                        tempo = Time.realtimeSinceStartup - tempoinicio;
                        Debug.Log("O tempo em GPU foi " + tempo);
                        btmGPUpressed = false;
                    }
                }

                

                computeBuffer.Dispose();


        }

        

    }
    

    void OnGUI()
    {
        if (data == null)
        {
            if (GUI.Button(new Rect(0, 0, 100, 50), "Create"))
            {
                createCube();
                btmCreatepressed = true;
                

            }
        }

        if (data != null)
        {
            if (GUI.Button(new Rect(110, 0, 100, 50), "Random CPU"))
            {
                for (int k = 0; k < iteractions; k++)
                {
                    for (int i = 0; i < gameObjects.Length; i++)
                    {
                        gameObjects[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
                        tempoinicio = Time.realtimeSinceStartup;
                        btmCPUpressed = true;
                    }
                }
            }
        }

        if (data != null)
        {
            if (GUI.Button(new Rect(220, 0, 100, 50), "Random GPU"))
            { 
                tempoinicio = Time.realtimeSinceStartup;
                int totalSize = 4 * sizeof(float) + 3 * sizeof(float) + 6* sizeof(float);

                btmGPUpressed = true;

                ComputeBuffer computeBuffer = new ComputeBuffer(data.Length, totalSize);


                computeShader.SetBuffer(0, "cubes", computeBuffer);
                computeShader.SetInt("iteraction", iteractions);
                computeBuffer.SetData(data);
                computeShader.Dispatch(0, data.Length / 10, 1, 1);

                computeBuffer.GetData(data);

                for (int i = 0; i < gameObjects.Length; i++)
                {
                    gameObjects[i].GetComponent<MeshRenderer>().material.SetColor("_Color", data[i].color);
                }

                computeBuffer.Dispose();
            }
        }

    }
    private void createCube()
    {
        data = new Cube[count * count];
        gameObjects = new GameObject[count * count];

        for (int i = 0; i < count; i++)
        {
            float offsetX = (-count / 2 + i);

            for (int j = 0; j < count; j++)
            {
                float offsetY = (-count / 2 + j);

                Color _color = Random.ColorHSV();

                GameObject go = GameObject.Instantiate(modelPref, new Vector3(offsetX * 0.7f, 0, offsetY * 0.7f), Quaternion.identity);
                go.GetComponent<MeshRenderer>().material.SetColor("_Color", _color);

                gameObjects[i * count + j] = go;

                data[i * count + j] = new Cube();
                data[i * count + j].position = go.transform.position;
                data[i * count + j].color = _color;
                data[i * count + j].gravidade = 9.8f;
            }
        }
    }
}
