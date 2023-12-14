using UnityEngine;
using TMPro;
using UnityEngine.AI;
using Platforms;
using System.Collections.Generic;

public class _GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _Grass;
    [SerializeField] private GameObject _Flowers1;
    [SerializeField] private GameObject _Flowers2;
    [SerializeField] private GameObject _Tree;
    [SerializeField] private GameObject _Fir;
    [SerializeField] private GameObject _Bush;
    [SerializeField] private GameObject _Fire;
    [SerializeField] private GameObject _Platform;
    [SerializeField] private GameObject _Altar;
    [SerializeField] private GameObject _Player;
    [SerializeField] private GameObject _Bear;
    private NavMeshSurface surface;
    private Transform _PlayerTransform;
    private GameObject[,] tileArray;
    private List<Transform> freeTileList = new List<Transform>();

    private int numberAltar = 9;


    private void OnDestroy()
    {
        EventBus.OnBearDied -= AddBear;
    }

    private Quaternion RandomRotation()
    {
        int Rotation = Random.Range(0, 4);
        float Y = 0f;
        switch (Rotation)
        {
            case 0:
                Y = 0f;
                break;
            case 1:
                Y = 90f;
                break;
            case 2:
                Y = 180f;
                break;
        }
        return Quaternion.Euler(0f, Y, 0f);
    }

    private void AddBear()
    {
        //float randomZ = Random.Range(2, 30) * 1.6f;
        //float randomX = Random.Range(2, 30) * 1.6f;
        //Vector3 spawnPoint = new Vector3(randomX, 1f, randomZ);
        //int h = (int)(randomX / 1.6f);
        //int w = (int)(randomZ / 1.6f);
        //Instantiate(_Bear, spawnPoint, Quaternion.identity);
        Vector3 spawnPoint = freeTileList[Random.Range(0, freeTileList.Count)].position;
        spawnPoint.y += 0.1f;
        Instantiate(_Bear, spawnPoint, Quaternion.identity);
    }

    private GameObject AddGrass(int _i, int _j)
    {
        GameObject Grass;
        Grass = Instantiate(_Grass, new Vector3(_i * 1.6f, 0f, _j * 1.6f), RandomRotation());
        return Grass;
    }

    private GameObject AddTree(int _i, int _j)
    {
        GameObject Tree;
        int Variant = Random.Range(0, 2);

        if (Variant == 0)
        {
            Tree = Instantiate(_Tree, new Vector3(_i * 1.6f, 0f, _j * 1.6f), Quaternion.identity);
        }
        else
        {
            Tree = Instantiate(_Fir, new Vector3(_i * 1.6f, 0f, _j * 1.6f), Quaternion.identity);
        }
        return Tree;
    }

    private GameObject AddFlowers(int _i, int _j, GameObject flowersType)
    {
        GameObject Flowers;
        Flowers = Instantiate(flowersType, new Vector3(_i * 1.6f, 0, _j * 1.6f), RandomRotation());
        return Flowers;
    }

    private GameObject AddBush(int _i, int _j)
    {
        GameObject Bush;
        Bush = Instantiate(_Bush, new Vector3(_i * 1.6f, 0f, _j * 1.6f), RandomRotation());
        return Bush;
    }

    private GameObject AddGround(int _i, int _j)
    {
        GameObject Ground = null;

        int _Variant = Random.Range(0, 3);

        switch (_Variant)
        {
            case 0:
                Ground = AddGrass(_i, _j);
                break;
            case 1:
                Ground = AddFlowers(_i, _j, _Flowers1);
                break;
            case 2:
                Ground = AddFlowers(_i, _j, _Flowers2);
                break;
        }
        freeTileList.Add(Ground.transform);
        return Ground;
    }

    private GameObject AddBerries(int _i, int _j)
    {
        GameObject Berry = null;

        int Probability = Random.Range(0, 17);

        switch (Probability)
        {
            case 0:
                Berry = AddBush(_i, _j);
                break;
            default:
                Berry = AddGround(_i, _j);
                break;
        }
        return Berry;
    }

    private GameObject AddAltar(int _i, int _j)
    {
        GameObject Altar = Instantiate(_Altar, new Vector3(_i * 1.6f, 0f, _j * 1.6f), Quaternion.identity);
        return Altar;
    }

    private void AddObjects(int h, int w)
    {
        tileArray = new GameObject[h,w];
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                bool exitZoneX = (i == (h / 2) - 2 || i == (h / 2) - 1 || i == (h / 2) || i == (h / 2) + 1);
                bool exitZoneY = (j == w - 1 || j == w - 2);
                bool saveZoneX = (i == (h / 2) - 3 || i == (h / 2) - 2 || i == (h / 2) - 1 || i == (h / 2) || i == (h / 2) + 1 || i == (h / 2) + 2 || i == (h / 2) + 3);
                bool saveZoneY = (j == (w / 2) - 3 || j == (w / 2) - 2 || j == (w / 2) - 1 || j == (w / 2) || j == (w / 2) + 1 || j == (w / 2) + 2 || j == (w / 2) + 3);
                bool I20 = i == (h / 4);
                bool I30 = i == (h / 2) - 10;
                bool I40 = i == (h / 2);
                bool I50 = i == (h / 2) + 10;
                bool I60 = i == h - (h / 4);

                bool J20 = j == (w / 4);
                bool J30 = j == (w / 2) - 10;
                bool J40 = j == (w / 2);
                bool J50 = j == (w / 2) + 10;
                bool J60 = j == w - (w / 4);


                bool altarCells = ((I20 && J60) || (I30 && J60) || (I40 && J60) || (I50 && J60) || (I60 && J60) ||
                                   (I20 && J50) ||                                                 (I60 && J50) ||
                                   (I20 && J40) ||                                                 (I60 && J40) ||
                                   (I20 && J30) ||                                                 (I60 && J30) ||
                                   (I20 && J20) || (I30 && J20) || (I40 && J20) || (I50 && J20) || (I60 && J20));



                if ((saveZoneX && saveZoneY) || (exitZoneX && exitZoneY))
                {
                    tileArray[i,j] = AddGround(i,j);
                }
                else
                {
                    if (altarCells)
                    {
                        numberAltar--;
                        tileArray[i, j] = AddAltar(i, j);
                        tileArray[i, j].GetComponentInChildren<TextMeshPro>().text = numberAltar.ToString();
                    }
                    else
                    {
                        int Variant = Random.Range(0, 3);

                        switch (Variant)
                        {
                            case 0:
                                tileArray[i, j] = AddTree(i, j);
                                break;
                            case 1:
                                tileArray[i, j] = AddBerries(i, j);
                                break;
                            default:
                                tileArray[i, j] = AddGround(i, j);
                                break;
                        }
                    }
                }
                tileArray[i, j].isStatic = true;
            }
        }
        GameObject Fire = Instantiate(_Fire, new Vector3(1.6f * (h / 2), 0.113f, 1.6f * (w / 2)), Quaternion.identity);
        Fire.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        _PlayerTransform.position = new Vector3(1.6f * (h / 2) + 3, 0.0777f, 1.6f * (w / 2) - 3);
        _PlayerTransform.rotation = Quaternion.Euler(0, -95f, 0);
    }


    private void Awake()
    {        
        _PlayerTransform = _Player.GetComponent<Transform>();
        AddObjects(40,40);
        surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
        EventBus.OnBearDied += AddBear;

        for (int i = 0; i < 15; i++)
        {            
            AddBear();
        }

        var _player = GameObject.Find("Man")?.GetComponent<PlayerBehaviour>();

        //Platform
        if (PlayerPrefs.HasKey("Platform"))
        {
            if (_player is not null)
                _player.platform = (Platform)PlayerPrefs.GetInt("Platform");
        }
        else
        {
            if (_player is not null)
                _player.platform = Platform.PC;
        }
    }
}
