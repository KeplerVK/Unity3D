using System.Collections;
using System.Collections.Generic;
using UnityEngine;


 
//导演对象
public class SSDirector : System.Object
{
    private static SSDirector _instance;
    public SceneController currentSceneController { get; set; }

    public static SSDirector getInstance()
    {
        if (_instance == null)
        {
            _instance = new SSDirector();
        }
        return _instance;
    }
}

//场记接口
public interface SceneController
{
    void loadResources();
}

//玩家操作接口
public interface UserAction
{
    void moveBoat();
    void characterIsClicked(MyCharacterController characterCont);
    void restart();
}

//动作类型定义
public enum SSActionEventType : int{Started, Competeted}

//事件处理接口
public interface ISSActionCallback
{
    void SSAtionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, Object objectParam = null);
}

//动作基类
public class SSAction : ScriptableObject {
    public bool enable = true;
    public bool destory = false;
    public GameObject gameObject {get; set;}
    public Transform transform {get; set;}
    public ISSActionCallback callback {get; set;}

    protected SSAction() {}

    public virtual void Start() {
        throw new System.NotImplementedException();
    }
    public virtual void Update() {
        throw new System.NotImplementedException();
    }
}

//简单动作实现
public class CCMoveToAction : SSAction {
    
     public Vector3 target;
    //public Vector3 target
    public float speed = 600;
    //public GameObject gameObject;
    
    public void setDestination(Vector3 target) {
        this.target = target;
        Update();
    }

    public static CCMoveToAction GetSSAtion(GameObject Object) {
        CCMoveToAction action = ScriptableObject.CreateInstance<CCMoveToAction> ();
        //action.target = target;
        action.gameObject = Object;
        action .transform = Object.transform;
        return action;
    }

    public void setGameObject(GameObject gameObject) {
        this.gameObject = gameObject;
        this.transform = gameObject.transform;
    }

    public override void Update() {
        
        //Debug.Log("go");
        //if(this.enable == false) {
        //    Debug.Log("false");
        //}
        this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed*Time.deltaTime);
        if(this.transform.position == target) {
            Debug.Log("getToTheDest");
            this.enable = false;
            //this.callback.SSAtionEvent(this);
        }
    }
}

public class CCGetBoatAction : SSAction  {
    public Vector3 target;
    //public Vector3 target
    public float speed = 500;
    //public GameObject gameObject;
    
    public void setDestination(Vector3 target) {
        this.target = target;
        Debug.Log(target.x);

        
        Debug.Log(this.transform.position.x);
        //this.enable = false;
        Update();
    }

    public static CCGetBoatAction GetSSAtion(GameObject Object) {
        CCGetBoatAction action = ScriptableObject.CreateInstance<CCGetBoatAction> ();
        //action.target = target;
        action.gameObject = Object;
        action .transform = Object.transform;
        return action;
    }

    public void setGameObject(GameObject gameObject) {
        this.gameObject = gameObject;
        this.transform = gameObject.transform;
    }

    public override void Update() {
        
        //Debug.Log("go");
        //if(this.enable == false) {
        //    Debug.Log("false");
        //}
        this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed*Time.deltaTime);
        if(this.transform.position == target) {
            Debug.Log("getToTheDest");
            this.enable = false;
            //this.callback.SSAtionEvent(this);
        }
    }
}

/*public class Moveable : MonoBehaviour
{

    CCGetBoatAction getBoatAction;
    

    // change frequently
    void Start() {
        getBoatAction = CCGetBoatAction.GetSSAtion();
    }

    void Update()
    {
        
    }
    public void setDestination(Vector3 _dest)
    {
        Debug.Log("setDest");
        getBoatAction.setDestination(_dest);
    }

    
}*/


/*-----------------------------------Moveable------------------------------------------*/
/*public class Moveable : MonoBehaviour
{

    readonly float moveSpeed = 20;

    // change frequently
    int movingStatus;   // 0->not moving, 1->moving to middle, 2->moving to dest
    Vector3 dest;
    Vector3 middle;

    void Update()
    {
        if (movingStatus == 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, middle, moveSpeed * Time.deltaTime);
            if (transform.position == middle)
            {
                movingStatus = 2;
            }
        }
        else if (movingStatus == 2)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, moveSpeed * Time.deltaTime);
            if (transform.position == dest)
            {
                movingStatus = 0;
            }
        }
    }
    public void setDestination(Vector3 _dest)
    {
        dest = _dest;
        middle = _dest;
        if (_dest.y == transform.position.y)
        {   // boat moving
            movingStatus = 2;
        }
        else if (_dest.y < transform.position.y)
        {   // character from coast to boat
            middle.y = transform.position.y;
        }
        else
        {                               // character from boat to coast
            middle.x = transform.position.x;
        }
        movingStatus = 1;
    }

    public void reset()
    {
        movingStatus = 0;
    }
}
*/

/*-----------------------------------MyCharacterController------------------------------------------*/
//组合动作实现
public class MyCharacterController
{
    readonly GameObject character;
    //readonly Moveable moveableScript;
    readonly ClickGUI clickGUI;
    readonly int characterType; // 0->priest, 1->devil

    readonly CCGetBoatAction getBoatAction;

    // change frequently
    public bool isOnBoat;
    CoastController coastController;


    public MyCharacterController(string theCharacter)
    {

        if (theCharacter == "priest")
        {
            character = Object.Instantiate(Resources.Load("Perfabs/Priest", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            characterType = 0;
        }
        else
        {
            character = Object.Instantiate(Resources.Load("Perfabs/Devil", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            characterType = 1;
        }
        //moveableScript = character.AddComponent(typeof(Moveable)) as Moveable;
        //moveToAction = character.AddComponent(typeof(CCMoveToAction)) as CCMoveToAction;
        
        getBoatAction = CCGetBoatAction.GetSSAtion(character);
        clickGUI = character.AddComponent(typeof(ClickGUI)) as ClickGUI;
        clickGUI.setController(this);
    }

    public void setName(string name)
    {
        character.name = name;
    }

    public void setPosition(Vector3 pos)
    {
        character.transform.position = pos;
    }

    public void moveToPosition(Vector3 destination)
    {
        //getBoatAction.setGameObject(character);
        getBoatAction.setDestination(destination);
       // moveableScript.setDestination(destination);
        
    }

    public int getType()
    {   // 0->priest, 1->devil
        return characterType;
    }

    public string getName()
    {
        return character.name;
    }

    public void getOnBoat(BoatController boatCtrl)
    {
        coastController = null;
        character.transform.parent = boatCtrl.getGameobj().transform;
        isOnBoat = true;
    }

    public void getOnCoast(CoastController coastCtrl)
    {
        coastController = coastCtrl;
        character.transform.parent = null;
        isOnBoat = false;
    }

    public bool getIsOnBoat()
    {
        return isOnBoat;
    }

    public CoastController getCoastController()
    {
        return coastController;
    }

    public void reset()
    {
        //moveableScript.reset();
        coastController = (SSDirector.getInstance().currentSceneController as FirstController).fromCoast;
        getOnCoast(coastController);
        setPosition(coastController.getEmptyPosition());
        coastController.getOnCoast(this);
    }
}

/*-----------------------------------CoastController------------------------------------------*/
public class CoastController
{
    readonly GameObject coast;
    readonly Vector3 from_pos = new Vector3(9, 1, 0);
    readonly Vector3 to_pos = new Vector3(-9, 1, 0);
    readonly Vector3[] positions;
    readonly int status;    // to->-1, from->1

    // change frequently
    MyCharacterController[] passengerPlaner;

    public CoastController(string _status)
    {
        positions = new Vector3[] {new Vector3(6.5F,2.25F,0), new Vector3(7.5F,2.25F,0), new Vector3(8.5F,2.25F,0),
                new Vector3(9.5F,2.25F,0), new Vector3(10.5F,2.25F,0), new Vector3(11.5F,2.25F,0)};

        passengerPlaner = new MyCharacterController[6];

        if (_status == "from")
        {
            coast = Object.Instantiate(Resources.Load("Perfabs/Stone", typeof(GameObject)), from_pos, Quaternion.identity, null) as GameObject;
            coast.name = "from";
            status = 1;
        }
        else
        {
            coast = Object.Instantiate(Resources.Load("Perfabs/Stone", typeof(GameObject)), to_pos, Quaternion.identity, null) as GameObject;
            coast.name = "to";
            status = -1;
        }
    }

    public int getEmptyIndex()
    {
        for (int i = 0; i < passengerPlaner.Length; i++)
        {
            if (passengerPlaner[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public Vector3 getEmptyPosition()
    {
        Vector3 pos = positions[getEmptyIndex()];
        pos.x *= status;
        return pos;
    }

    public void getOnCoast(MyCharacterController characterCtrl)
    {
        int index = getEmptyIndex();
        passengerPlaner[index] = characterCtrl;
    }

    public MyCharacterController getOffCoast(string passenger_name)
    {   // 0->priest, 1->devil
        for (int i = 0; i < passengerPlaner.Length; i++)
        {
            if (passengerPlaner[i] != null && passengerPlaner[i].getName() == passenger_name)
            {
                MyCharacterController charactorCtrl = passengerPlaner[i];
                passengerPlaner[i] = null;
                return charactorCtrl;
            }
        }
        Debug.Log("cant find passenger on coast: " + passenger_name);
        return null;
    }

    public int getStatus()
    {
        return status;
    }

    public int[] getCharacterNum()
    {
        int[] count = { 0, 0 };
        for (int i = 0; i < passengerPlaner.Length; i++)
        {
            if (passengerPlaner[i] == null)
                continue;
            if (passengerPlaner[i].getType() == 0)
            {   // 0->priest, 1->devil
                count[0]++;
            }
            else
            {
                count[1]++;
            }
        }
        return count;
    }

    public void reset()
    {
        passengerPlaner = new MyCharacterController[6];
    }
}

/*-----------------------------------BoatController------------------------------------------*/
public class BoatController
{
    readonly GameObject boat;
    //readonly Moveable moveableScript;
    readonly Vector3 fromPosition = new Vector3(5, 1, 0);
    readonly Vector3 toPosition = new Vector3(-5, 1, 0);
    readonly Vector3[] fromPositions;
    readonly Vector3[] toPositions;

    readonly CCMoveToAction moveToAction;
    

    // change frequently
    int status; // to->-1; from->1
    MyCharacterController[] passenger = new MyCharacterController[2];

    public BoatController()
    {
        status = 1;

        fromPositions = new Vector3[] { new Vector3(4.5F, 1.5F, 0), new Vector3(5.5F, 1.5F, 0) };
        toPositions = new Vector3[] { new Vector3(-5.5F, 1.5F, 0), new Vector3(-4.5F, 1.5F, 0) };

        boat = Object.Instantiate(Resources.Load("Perfabs/Boat", typeof(GameObject)), fromPosition, Quaternion.identity, null) as GameObject;
        boat.name = "boat";

        moveToAction = CCMoveToAction.GetSSAtion(boat);
       

        //moveableScript = boat.AddComponent(typeof(Moveable)) as Moveable;
        boat.AddComponent(typeof(ClickGUI));
    }


    public void Move()
    {
        if (status == -1)
        {
            //moveableScript.setDestination(fromPosition);
            moveToAction.setDestination(fromPosition);
            status = 1;
        }
        else
        {
            moveToAction.setDestination(toPosition);
            //moveableScript.setDestination(toPosition);
            status = -1;
        }
    }

    public int getEmptyIndex()
    {
        for (int i = 0; i < passenger.Length; i++)
        {
            if (passenger[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public bool isEmpty()
    {
        for (int i = 0; i < passenger.Length; i++)
        {
            if (passenger[i] != null)
            {
                return false;
            }
        }
        return true;
    }

    public Vector3 getEmptyPosition()
    {
        Vector3 pos;
        int emptyIndex = getEmptyIndex();
        if (status == -1)
        {
            pos = toPositions[emptyIndex];
        }
        else
        {
            pos = fromPositions[emptyIndex];
        }
        return pos;
    }

    public void GetOnBoat(MyCharacterController characterCtrl)
    {
        int index = getEmptyIndex();
        passenger[index] = characterCtrl;
    }

    public MyCharacterController GetOffBoat(string passenger_name)
    {
        for (int i = 0; i < passenger.Length; i++)
        {
            if (passenger[i] != null && passenger[i].getName() == passenger_name)
            {
                MyCharacterController charactorCtrl = passenger[i];
                passenger[i] = null;
                return charactorCtrl;
            }
        }
        Debug.Log("Cant find passenger in boat: " + passenger_name);
        return null;
    }

    public GameObject getGameobj()
    {
        return boat;
    }

    public int getStatus()
    { // to->-1; from->1
        return status;
    }

    public int[] getCharacterNum()
    {
        int[] count = { 0, 0 };
        for (int i = 0; i < passenger.Length; i++)
        {
            if (passenger[i] == null)
                continue;
            if (passenger[i].getType() == 0)
            {   // 0->priest, 1->devil
                count[0]++;
            }
            else
            {
                count[1]++;
            }
        }
        return count;
    }

    public void reset()
    {
        //moveableScript.reset();
        if (status == -1)
        {
            Move();
        }
        passenger = new MyCharacterController[2];
    }
}
