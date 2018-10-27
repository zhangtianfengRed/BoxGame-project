using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class Map : MonoBehaviour
{

    public int distance;
    public Sprite[] Button;
    internal GameObject Mouse_Gameobject;
    Direction direction = new Direction();
    public GameObject Tile_backGround;
    public GameObject Tile_Button;
    public GameObject Tile_Target;
    public GameObject Tile_Box;
    public GameObject Box_mapData;
    public GameObject Target_mapData;
    public GameObject[] Moveanim = new GameObject[4];

    //临时测试地图
    public GameObject[] map = new GameObject[4];
    mMapData mMapData;

    BoxMove boxmove = new BoxMove();


    public delegate void ButtonDown(GameObject gameobjet, Vector2 vector);
    public event ButtonDown OnButton;

    // Use this for initialization
    void Start()
    {
        OnButton += Animoveinstance;
        mMapData = new mMapData(distance)
        {
            tile_BackGround = Tile_backGround,
            tile_Button = Tile_Button,
            tile_Target = Tile_Target,
            tile_Box = Tile_Box
        };
        GameObject T = Instantiate(map[Random.Range(0, 4)], new Vector3(0, 0, 0), Quaternion.identity);
        StartMap();

        mMapData.ForeachBox(T);
    }

    // Update is called once per frame
    void Update()
    {
        if (boxmove.Move())
        {
            boxmove.BoxMoveAnim();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray();
            if (Mouse_Gameobject != null)
            {
                Sprite_change(Mouse_Gameobject, Button[1]);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Mouse_Gameobject != null)
            {
                Sprite_change(Mouse_Gameobject, Button[0]);
                Push();
                Mouse_Gameobject = null;
                direction._Mouse_Gameobject = null;
            }

        }
    }


    //方向判定
    struct Direction
    {
        public GameObject _Mouse_Gameobject { get; set; }
        internal bool directionX(int T)
        {
            Vector2Int Getbutton = Vis.Floattoint(_Mouse_Gameobject.transform.position);

            if (Getbutton.x == T)
            {
                return true;
            }
            return false;
        }
        internal bool directionY(int T)
        {
            Vector2Int Getbutton = Vis.Floattoint(_Mouse_Gameobject.transform.position);

            if (Getbutton.y == T)
            {
                return true;
            }
            return false;
        }
    }

    struct RayDirection
    {
        public Vector2Int direction { get; set; }
        public RayDirection(Vector2Int T)
        {
            direction = T;
        }
    }
    //根据点击方向遍历
    void Push()
    {
        //RayDirection rayDirection=new RayDirection(Vector2Int.zero);
        if (direction.directionX(mMapData.StartVector.x))
        {
            OnButton(Moveanim[3], new Vector2(direction._Mouse_Gameobject.transform.position.x + 0.5f, direction._Mouse_Gameobject.transform.position.y));
            StartCoroutine(StartMoveRight());          
        }
        if (direction.directionX(mMapData.MapWidth))
        {
            OnButton(Moveanim[2], new Vector2(direction._Mouse_Gameobject.transform.position.x - 0.5f, direction._Mouse_Gameobject.transform.position.y));
            StartCoroutine(StartMoveLeft());
        }
        if (direction.directionY(mMapData.StartVector.y))
        {
            OnButton(Moveanim[0], new Vector2(direction._Mouse_Gameobject.transform.position.x , direction._Mouse_Gameobject.transform.position.y+0.5f));
            StartCoroutine(StartMoveUP());
        }
        if (direction.directionY(mMapData.MapHigh))
        {
            OnButton(Moveanim[1], new Vector2(direction._Mouse_Gameobject.transform.position.x, direction._Mouse_Gameobject.transform.position.y - 0.5f));
            StartCoroutine(StartMoveDown());
        }
        TargetFull();
    }

    void Animoveinstance(GameObject T ,Vector2 V) {
        GameObject v= Instantiate(T, V, Quaternion.identity);
        v.AddComponent<Destroeanim>();
    }
    //目标物体都放到了需要的位置上
    void TargetFull()
    {
        if (mMapData.TargetCheck())
        {
            SceneManager.LoadScene(0);
        }
    }
    //朝右推动时返回最远距离的Gameobject
    GameObject LastGameobjet()
    {
        
        for (int i = mMapData.Distance - 1; i > -1; i -= 1)
        {
            if (mMapData.mapData[i, buttonObjectright.Y_axis] != null)
            {
                Vector2Int V = Vis.Floattoint(mMapData.mapData[i, buttonObjectright.Y_axis].transform.position);
                if (FindPostion(V, new Vector2Int(V.x + 1, V.y)) != V)
                {
                    return mMapData.mapData[i, buttonObjectright.Y_axis];
                }
            }
        }
        return null;
    }
    //向右判定有没有空的位置
    Vector2Int FindPostion(Vector2Int Thepostion, Vector2Int Nrepostion)
    {
        if (Nrepostion.x < mMapData.MapWidth)
        {
            if (mMapData.mapData[Nrepostion.x, Nrepostion.y] == null)
            {
                return Nrepostion;
            }
            return FindPostion(Thepostion, new Vector2Int(Nrepostion.x + 1, Nrepostion.y));
        }
        return Thepostion;
    }
    //迭代推送
    void iteratePostion(Vector2Int Newpostion, Vector2Int Thepostion)
    {
        //获取新的坐标
        mMapData.mapData[Newpostion.x - 1, Newpostion.y].transform.position = new Vector3(Newpostion.x, Newpostion.y, 0);
        mMapData.mapData[Newpostion.x, Newpostion.y] = mMapData.mapData[Newpostion.x - 1, Newpostion.y];
        mMapData.mapData[Newpostion.x - 1, Newpostion.y] = null;
    }
    //朝右运动
    void MoveRight()
    {
        if (LastGameobjet() != null)
        {
            var Thepostion = Vis.Floattoint(LastGameobjet().transform.position);
            //获取新的坐标
            Vector2Int Newpostion = FindPostion(Thepostion, new Vector2Int(Thepostion.x + 1, Thepostion.y));
            if (Mathf.Abs(Thepostion.x - Newpostion.x) <= 1)
            {
                if (Thepostion != Newpostion)
                {
                    mMapData.mapData[Thepostion.x, Thepostion.y].transform.position = new Vector3(Newpostion.x, Newpostion.y, 0);
                    mMapData.mapData[Newpostion.x, Newpostion.y] = mMapData.mapData[Thepostion.x, Thepostion.y];
                    mMapData.mapData[Thepostion.x, Thepostion.y] = null;
                }
            }
            if (Mathf.Abs(Thepostion.x - Newpostion.x) > 1)
            {
                iteratePostion(Newpostion, Thepostion);
            }
        }
    
    }

    ButtonObject buttonObjectright, buttonObjectleft, buttonObjectdown, buttonObjectup;
    //开始朝右移动
    IEnumerator StartMoveRight()
    {
        Vector2Int T = Vis.Floattoint(direction._Mouse_Gameobject.transform.position);
        buttonObjectright = new ButtonObject()
        {
            Y_axis = T.y  
        };

      for (int i = 0; i < (mMapData.Distance * mMapData.Distance); i++)
     {
            if (LastGameobjet() != null)
            {
                yield return new WaitForSeconds(0.01f);
                MoveRight();
            }
                //mMapData.UpdateMapData(Box_mapData);
      }
        
      
    }


    //朝左推动时返回最远距离的Gameobjt
    GameObject LastGameobjet_Left()
    {
        for (int i = mMapData.MapWidth - mMapData.Distance; i < mMapData.MapWidth; i += 1)
        {
            if (mMapData.mapData[i, buttonObjectleft.Y_axis] != null)
            {
                Vector2Int V = Vis.Floattoint(mMapData.mapData[i, buttonObjectleft.Y_axis].transform.position);
                if (FindPostionLeft(V, new Vector2Int(V.x - 1, V.y)) != V)
                {
                    return mMapData.mapData[i, buttonObjectleft.Y_axis];
                }

            }
        }
        return null;
    }
    //向左判定有没有空的位置
    Vector2Int FindPostionLeft(Vector2Int Thepostion, Vector2Int Nrepostion)
    {
        if (Nrepostion.x > -1)
        {
            if (mMapData.mapData[Nrepostion.x, Nrepostion.y] == null)
            {
                return Nrepostion;
            }
            return FindPostionLeft(Thepostion, new Vector2Int(Nrepostion.x - 1, Nrepostion.y));
        }
        return Thepostion;
    }
    //根据坐标进行移动
    void iteratePostionleft(Vector2Int Newpostion, Vector2Int Thepostion)
    {
        //获取新的坐标
        mMapData.mapData[Newpostion.x + 1, Newpostion.y].transform.position = new Vector3(Newpostion.x, Newpostion.y, 0);
        mMapData.mapData[Newpostion.x, Newpostion.y] = mMapData.mapData[Newpostion.x + 1, Newpostion.y];
        mMapData.mapData[Newpostion.x + 1, Newpostion.y] = null;
    }
    //朝左运动
    void MoveLeft()
    {
        if (LastGameobjet_Left() != null)
        {
            var Thepostion = Vis.Floattoint(LastGameobjet_Left().transform.position);
            //获取新的坐标
            Vector2Int Newpostion = FindPostionLeft(Thepostion, new Vector2Int(Thepostion.x - 1, Thepostion.y));
            if (Mathf.Abs(Thepostion.x - Newpostion.x) <= 1)
            {
                if (Thepostion != Newpostion)
                {
                    mMapData.mapData[Thepostion.x, Thepostion.y].transform.position = new Vector3(Newpostion.x, Newpostion.y, 0);
                    mMapData.mapData[Newpostion.x, Newpostion.y] = mMapData.mapData[Thepostion.x, Thepostion.y];
                    mMapData.mapData[Thepostion.x, Thepostion.y] = null;
                }
            }
            if (Mathf.Abs(Thepostion.x - Newpostion.x) > 1)
            {
                iteratePostionleft(Newpostion, Thepostion);
            }
        }
    }
    //开始朝左移动
    IEnumerator StartMoveLeft()
    {
        Vector2Int T = Vis.Floattoint(direction._Mouse_Gameobject.transform.position);
        buttonObjectleft = new ButtonObject()
        {
            Y_axis = T.y
        };
        if (direction._Mouse_Gameobject != null)
        {
            for (int i = 0; i < (mMapData.Distance * mMapData.Distance); i++)
            {
                yield return new WaitForSeconds(0.01f);
                MoveLeft();               
            }
        }
    }
    //朝上运动
    //朝上推动时返回最远距离的Gameobject
    GameObject UPGameobjet()
    {
        for (int i = mMapData.Distance - 1; i > -1; i--)
        {
            if (mMapData.mapData[buttonObjectup.X_axis, i] != null)
            {
                Vector2Int V = Vis.Floattoint(mMapData.mapData[buttonObjectup.X_axis, i].transform.position);
                if (FindPostionUP(V, new Vector2Int(V.x, V.y + 1)) != V)
                {
                    return mMapData.mapData[buttonObjectup.X_axis, i];
                }
            }
        }
        return null;
    }
    //获取上移的坐标
    Vector2Int FindPostionUP(Vector2Int Thepostion, Vector2Int Nrepostion)
    {
        if (Nrepostion.y < mMapData.MapHigh)
        {
            if (mMapData.mapData[Nrepostion.x, Nrepostion.y] == null)
            {
                return Nrepostion;
            }
            return FindPostionUP(Thepostion, new Vector2Int(Nrepostion.x, Nrepostion.y + 1));
        }
        return Thepostion;
    }

    //根据坐标进行移动
    void iteratePostionUP(Vector2Int Newpostion, Vector2Int Thepostion)
    {
        //获取新的坐标
        mMapData.mapData[Newpostion.x , Newpostion.y-1].transform.position = new Vector3(Newpostion.x, Newpostion.y, 0);
        mMapData.mapData[Newpostion.x, Newpostion.y] = mMapData.mapData[Newpostion.x , Newpostion.y - 1];
        mMapData.mapData[Newpostion.x , Newpostion.y -1] = null;
    }
    //朝上运动
    void MoveUP()
    {
        if (UPGameobjet() != null)
        {
            var Thepostion = Vis.Floattoint(UPGameobjet().transform.position);
            //获取新的坐标
            Vector2Int Newpostion = FindPostionUP(Thepostion, new Vector2Int(Thepostion.x , Thepostion.y + 1));
            if (Mathf.Abs(Thepostion.y - Newpostion.y) <= 1)
            {
                if (Thepostion != Newpostion)
                {
                    mMapData.mapData[Thepostion.x, Thepostion.y].transform.position = new Vector3(Newpostion.x, Newpostion.y, 0);
                    mMapData.mapData[Newpostion.x, Newpostion.y] = mMapData.mapData[Thepostion.x, Thepostion.y];
                    mMapData.mapData[Thepostion.x, Thepostion.y] = null;
                }
            }
            if (Mathf.Abs(Thepostion.y - Newpostion.y) > 1)
            {
                iteratePostionUP(Newpostion, Thepostion);
            }
        }
    }
    //开始朝上移动
    IEnumerator StartMoveUP()
    {
        Vector2Int T = Vis.Floattoint(direction._Mouse_Gameobject.transform.position);
        buttonObjectup = new ButtonObject()
        {
            X_axis = T.x
        };
        if (UPGameobjet() != null)
        {
            for (int i = 0; i < (mMapData.Distance * mMapData.Distance); i++)
            {
                yield return new WaitForSeconds(0.01f);
                MoveUP();
            }
        }
    }
    //朝下运动
    //朝下推动时返回最远距离的Gameobjt
    GameObject Downameobjet()
    {
        for (int i = mMapData.Distance; i < mMapData.MapHigh; i++)
        {
            if (mMapData.mapData[buttonObjectdown.X_axis, i] != null)
            {
                Vector2Int V = Vis.Floattoint(mMapData.mapData[buttonObjectdown.X_axis, i].transform.position);
                if (FindPostionDown(V, new Vector2Int(V.x, V.y - 1)) != V)
                {
                    return mMapData.mapData[buttonObjectdown.X_axis, i];
                }
            }
        }
        return null;
    }
    Vector2Int FindPostionDown(Vector2Int Thepostion, Vector2Int Nrepostion)
    {
        if (Nrepostion.y > -1)
        {
            if (mMapData.mapData[Nrepostion.x, Nrepostion.y] == null)
            {
                return Nrepostion;
            }
            return FindPostionDown(Thepostion, new Vector2Int(Nrepostion.x, Nrepostion.y - 1));
        }
        return Thepostion;
    }
    //根据坐标进行移动
    void iteratePostionDown(Vector2Int Newpostion, Vector2Int Thepostion)
    {
        //获取新的坐标
        mMapData.mapData[Newpostion.x, Newpostion.y + 1].transform.position = new Vector3(Newpostion.x, Newpostion.y, 0);
        mMapData.mapData[Newpostion.x, Newpostion.y] = mMapData.mapData[Newpostion.x, Newpostion.y + 1];
        mMapData.mapData[Newpostion.x, Newpostion.y+ 1] = null;
    }
    //朝下运动
    void MoveDown()
    {
        if (Downameobjet() != null)
        {
            var Thepostion = Vis.Floattoint(Downameobjet().transform.position);
            //获取新的坐标
            Vector2Int Newpostion = FindPostionDown(Thepostion, new Vector2Int(Thepostion.x, Thepostion.y - 1));
            if (Mathf.Abs(Thepostion.y - Newpostion.y) <= 1)
            {
                if (Thepostion != Newpostion)
                {
                    mMapData.mapData[Thepostion.x, Thepostion.y].transform.position = new Vector3(Newpostion.x, Newpostion.y, 0);
                    mMapData.mapData[Newpostion.x, Newpostion.y] = mMapData.mapData[Thepostion.x, Thepostion.y];
                    mMapData.mapData[Thepostion.x, Thepostion.y] = null;
                }
            }
            if (Mathf.Abs(Thepostion.y - Newpostion.y) > 1)
            {
                iteratePostionDown(Newpostion, Thepostion);
            }
        }
    }
    //开始朝下移动
    IEnumerator StartMoveDown()
    {
        Vector2Int T = Vis.Floattoint(direction._Mouse_Gameobject.transform.position);
        buttonObjectdown = new ButtonObject()
        {
            X_axis = T.x
        };
        if (Downameobjet() != null)
        {
            for (int i = 0; i < (mMapData.Distance * mMapData.Distance); i++)
            {
                yield return new WaitForSeconds(0.01f);
                MoveDown();
                //mMapData.UpdateMapData(Box_mapData);
            }
        }
    }
    //射线检测按钮
    void Ray()
    {
        Vector3 Mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast(Mouse, Vector3.zero);
        if (hit2D.collider != null && hit2D.collider.gameObject.tag == "Button")
        {
            Mouse_Gameobject = hit2D.collider.gameObject;
            direction._Mouse_Gameobject = Mouse_Gameobject;
        }
    }

    void Sprite_change(GameObject a, Sprite b)
    {
        a.GetComponent<SpriteRenderer>().sprite = b;
    }

    void StartMap()
    {
        GameObject mapparent = new GameObject("mapparent");
        GameObject Buttonparent = new GameObject("Buttonparent");

        for (int x = 0; x < mMapData.MapWidth; x++)
        {
            for (int y = 0; y < mMapData.MapHigh; y++)
            {
                GameObject T = Instantiate(mMapData.tile_BackGround, new Vector3Int(x, y, 1), Quaternion.identity);
                T.transform.parent = mapparent.transform;

            }
        }
        for (int x = -1; x < mMapData.MapWidth + 1; x++)
        {
            for (int y = -1; y < mMapData.MapHigh + 1; y++)
            {
                if (x == -1 || x == mMapData.MapWidth)
                {
                    GameObject T = Instantiate(mMapData.tile_Button, new Vector3Int(x, y, 0), Quaternion.identity);
                    T.transform.parent = Buttonparent.transform;
                    T.layer = 8;
                }
                if (y == -1 || y == mMapData.MapHigh)
                {
                    GameObject T = Instantiate(mMapData.tile_Button, new Vector3Int(x, y, 0), Quaternion.identity);
                    T.transform.parent = Buttonparent.transform;
                    T.layer = 8;
                }


            }
        }

        Camera.main.transform.position = new Vector3((mMapData.MapWidth - 1) / 2, (mMapData.MapHigh - 1) / 2, -10);

    }


}

class mMapData
{
    public int MapWidth { get; set; }
    public int MapHigh { get; set; }
    public int Distance { get; set; }
    public GameObject tile_BackGround { get; set; }
    public GameObject tile_Button { get; set; }
    public GameObject tile_Target { get; set; }
    public GameObject tile_Box { get; set; }


    public GameObject[,] mapData;
    public GameObject[,] Target;

    //地图初始位置，计算按钮攻击方向
    internal Vector2Int StartVector;

    public mMapData(int _distance)
    {
        Distance = _distance;
        MapWidth = Distance * 2;
        MapHigh = Distance * 2;
        mapData = new GameObject[MapWidth, MapHigh];
        Target = new GameObject[MapWidth, MapHigh];
        StartVector = new Vector2Int(-1, -1);
    }
    public enum BoxTag
    {
        //地点为空
        None = 0,
        //放有普通盒子
        Box = 1,
        //放有目标点位置
        Target = 2,
        //目标盒子的标签
        Box_Target = 3
    }
    //初始化遍历各个物体的位置
    public void ForeachBox(GameObject Box)
    {
        foreach (Transform i in Box.GetComponentInChildren<Transform>())
        {
            if (i.tag == BoxTag.Box.ToString() || i.tag == BoxTag.Box_Target.ToString())
            {
                Vector2Int T = Vis.Floattoint(i.position);
                mapData[T.x, T.y] = i.gameObject;
            }
            if (i.tag == BoxTag.Target.ToString())
            {
                Vector2Int T = Vis.Floattoint(i.position);
                Target[T.x, T.y] = i.gameObject;
            }
        }
    }
    //检查地图中目标位置是否都已经放置了目标
    internal bool TargetCheck()
    {
        foreach (GameObject i in Target)
        {
            if (i != null)
            {
                Vector2Int T = Vis.Floattoint(i.transform.position);
                if (mapData[T.x, T.y] == null || mapData[T.x, T.y].tag != BoxTag.Box_Target.ToString())
                {
                    return false;
                }
            }
        }
        return true;
    }
}

class BoxMove
{
    public GameObject Thegame { get; set; }
    public Vector3 Target { get; set; }

    internal bool Move()
    {
        if (Thegame != null)
        {
            return true;
        }
        return false;
    }
    internal void BoxMoveAnim()
    {
        Thegame.transform.position = Vector3.Lerp(Thegame.transform.position, Target, 2f);


    }
}

static class Vis
{

    internal static float pushtime = 1;
    internal static Vector2Int Floattoint(Vector2 T)
    {
        return new Vector2Int(Mathf.FloorToInt(T.x), Mathf.FloorToInt(T.y));
    }
}
class ButtonObject
{
    public int X_axis { get; set; }
    public int Y_axis { get; set; }
    public int LastPosion { get; set; }
}

