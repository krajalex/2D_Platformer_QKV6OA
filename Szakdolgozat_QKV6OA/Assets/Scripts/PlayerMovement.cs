using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Ez itt a PlayerController-�nk, itt ir�ny�tjuk/hat�rozzuk meg a karaker mozg�s�t, k�l�b�z? tulajdons�gait.
/// Itt tal�lhat� m�g egy groundCheck gameobject-re mutat� referencia is, amely megn�zi, hogy a karakter a f�ld�n van-e ezt egy groundCheckRadius nev? sugaron bel�l teszi. Itt van m�g egy LayerMask is, ami az�rt kell, hogy csak azokat
/// n�zze amelyeknek a Ground Layer-re van �ll�tva. Az isTouchningGround v�ltoz� azt n�zi meg, hogy a karakter �ppen hozz��r-e a f�ldh�z.
/// A respawnpoint t�rolja azokat a kordin�t�kat, ahova a karakter ker�l miut�n meghalt.
/// A fallDetector �rz�keli, ha a karakter kiesik a p�ly�r�l. A lifeCounter az �ppen el�rhet� �letet t�rolja el.
/// A handpositionchecker gameobject-re mutat� referencia egy statikus object a p�ly�n, amihez k�pest majd a Kinect �ltal �rz�kelt kezeket viszony�tjuk, amelyek a left �s right GameObjectben lesznek elt�rolva.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    //variables section 1
    private string MoveType; //A Kinect ir�ny�t�s�hoz sz�ks�ges v�ltoz�, amely a karakter pontos mozg�s�nak t�pus�t t�rolja el (pl.: kinect_move_left_and_jump)

    private int lifeCounter; //Defini�lja a karakter fennmarad� �leteinek sz�m�t

    private float normalCam; //A felhaszn�l� sz�m�ra l�that� teret mutat� kamera l�t�sz�ge/m�rete

    private Rigidbody2D playerBody; //A karaktert, a Player gameobject Rigidbody 2D komponens�re hivatkozva lehets�ges mozg�sra b�rni, ez�rt hivatkozunk r� a k�dban
    private Animator playerAnimator; //A karaktert, a Player gameobject Animator komponens�re hivatkozva lehets�ges meganim�lni, ez�rt hivatkozunk r� a k�dban
    private Vector3 respawnPoint; //A karakter �jra�led�s�nek hely�t t�rolja el

    public static int finalScore; //Az adott p�lya teljes�t�sekor megjelen? �sszpontsz�m
    public static int lastSceneNumber; //A legut�bb j�tszott scene buildsz�m�t t�rolja el (Sz�ks�ges a Congrats scene-hez �s script-hez)

    public int countdownTime; //A karakter �jra�led�sekor eltelo visszasz�ml�l� idotartam�t t�rolja el, mielott a felhaszn�l� �jra tudja ir�ny�tani a karaktert

    public GameObject fallDetector; //�rz�keli, ha a karakter leesik a p�ly�r�l
    public GameObject TrapTiles; //A 3. szinten l�vo csapd�k feletti f�ld elemek sz�lo gameobject-j�re mutat� referenci�t t�rolja el
    public GameObject Traps; //A 3. szinten l�vo csapd�k sz�lo gameobject-j�re mutat� referenci�t t�rolja el
    public GameObject handpositionchecker; //A Kinect ir�ny�t�s�hoz sz�ks�ges v�ltoz�, amely a kezek aktu�lis poz�ci�j�t defini�lja
    public GameObject left; //A Kinect ir�ny�t�s�hoz sz�ks�ges v�ltoz�, amely a bal k�z aktu�lis poz�ci�j�t hat�rozza meg
    public GameObject right; //A Kinect ir�ny�t�s�hoz sz�ks�ges v�ltoz�, amely a jobb k�z aktu�lis poz�ci�j�t hat�rozza meg

    public Text lifeText; //A karakter h�tral�vo �leteinek sz�m�t jelen�ti meg
    public Text countdownDisplay; //A karakter �jra�led�sekor megjeleno visszasz�ml�l�t jelen�ti meg
    public Camera cam; //A felhaszn�l� sz�m�ra l�that� teret mutat� kamera
    public Timer script; //A visszasz�ml�l� k�dj�nak referenci�ja
    public AudioSource checkpointSound; //A checkpoint-ok felv�telekor lej�tsz�d� hangeffekt referenci�ja
    public AudioSource dieSound; //A karakter hal�lakor lej�tsz�d� hangeffekt referenci�ja

    //variables section 2
    private float movementInputDirection; //Ebben a v�ltoz�ban t�roljuk el, hogy a karakter �ppen melyik ir�nyba mozog (-1, 0, vagy +1 �rt�ket ad vissza)
    private float jumpTimer;
    private float turnTimer;

    private int amountOfJumpsLeft; //A marad�k ugr�sok sz�m�t t�rolja el
    private int facingDirection = 1; //A karakter aktu�lis ir�ny�t t�rolja el -1 vagy +1 form�ban. Kezdetben az�rt 1 mert a p�ly�k elindul�sakor a karakter mindig jobbra n�z

    private bool isFacingRight = true; //Ebben a v�ltoz�ban t�roljuk el, hogy a karakter �ppen j� ir�nyba n�z-e. Amikor �jra�led, akkor mindig j� ir�nyba n�z, ez�rt alap�rtelmezetten true �rt�ku
    private bool isWalking; //Ez a v�ltoz� elt�rolja, hogy a karakter �ppen a f�ld�n s�t�l vagy nem
    private bool isGrounded; //Ez a v�ltoz� elt�rolja, hogy a karakter �ppen a f�ld�n van-e
    private bool canNormalJump; //Elt�rolja, hogy a karakter �ppen ugorhat-e norm�l m�don vagy sem
    private bool isAttemptingToJump; //Elt�rolja, hogy a karakter �ppen ugrani pr�b�l-e (A felhaszn�l� bevitele alapj�n)
    private bool checkJumpMultiplier;
    private bool canMove; //Elt�rolja, hogy a karakter �ppen mozoghat-e
    private bool canFlip; //Elt�rolja, hogy a karakter �ppen megfordulhat-e

    public int amountOfJumps = 1; //Defini�lja, hogy a karakter h�nyszor k�pes ugrani addig, am�g �jra a f�ldh�z nem �r

    public float movementSpeed = 10.0f; //A karakter mozg�si sebess�g�t hat�rozza meg
    public float jumpForce = 16.0f; //A karakter ugr�sakor a karakter Y tengely szerinti gyorsul�s�nak �rt�ke, teh�t az ugr�s�nak ereje
    public float groundCheckRadius; //A karakter k�r�li sug�r, ami �rz�keli, hogy a karakter a f�ld�n van-e
    public float movementForceInAir; //A karakterre hat� ero nagys�ga, ha a levegoben ir�ny�tja a felhaszn�l�
    public float airDragMultiplier = 0.95f; //A karakterre hat� l�gellen�ll�s ereje, amikor a levegoben kap bevitelt a felhaszn�l�t�l
    public float variableJumpHeightMultiplier = 0.5f; //Arra szolg�l, hogy figyelembe vegye a felhaszn�l� �ltal lenyomott ugr�si billentyu lenyom�si idej�t, ez�ltal befoly�solva, hogy milyen magasra ugrik fel a karakter
    public float jumpTimerSet = 0.15f;

    public Transform groundCheck; //Ez v�ltoz� hivatkozik a GroundCheck gameobject-re, amivel ellenorizz�k, hogy a karakter a f�ld�n van-e �ppen

    public LayerMask whatIsGround; //Ez a v�ltoz� defini�lja a talajt/f�ldet

    public int nextSceneLoad; //A k�vetkezo p�lya index�t t�rolja el

    // Start is called before the first frame update
    /// <summary>
    /// A j�t�k indit�sakor a player v�ltoz� megkapja a player objektumot, a playerAnimation pedig az animator komponenst. A respawnPoint �rt�ke a player alap poz�ci�j�ra �ll be.
    /// Az kezd��leter� 3.
    /// A normalCam pedig a Camera kezd� zoom �rt�ket t�rolja.
    /// </summary>
    void Start()
    {
        //Apply graphics settings
        QualitySettings.SetQualityLevel(SettingsMenu.qualityIndex_Out);

        //Gameobject Komponensek referenci�inak elt�rol�sa
        playerBody = GetComponent<Rigidbody2D>(); //Ezzel t�rolunk el egy referenci�t a player Rigidbody 2D komponens�re, illetve itt kapja meg a rigidbody2D v�ltoz� a Player gameobject elso olyan komponens�t,
                                                  //aminek Rigidbody 2D a t�pusa, a k�d elindul�sakor
        playerAnimator = GetComponent<Animator>(); //Ezzel t�rolunk el egy referenci�t a player Animator komponens�re, illetve itt kapja meg az animator v�ltoz� a Player gameobject elso olyan komponens�t,
                                                   //aminek Animator a t�pusa, a k�d elindul�sakor

        //V�ltoz�k kezdeti �rt�keinek be�ll�t�sa
        amountOfJumpsLeft = amountOfJumps; //A marad�k ugr�sok sz�m�nak alap�rt�kre �ll�t�sa
        respawnPoint = transform.position; //A karakter alap�rtelmezett �jra�led�s�nek poz�ci�ja a kezd?poz�ci�
        lifeCounter = 3; //A karakter alap�rtelmezett �leteinek sz�m�t hat�rozza meg
        normalCam = cam.orthographicSize; //A k�d indul�sakor a kamera f�kusz�nak alap�rtelmezettre �ll�t�sa
        nextSceneLoad = SceneManager.GetActiveScene().buildIndex + 1; //A k�vetkezo p�lya index�t hat�rozza meg
        lastSceneNumber = SceneManager.GetActiveScene().buildIndex; //A jelenlegi p�lya index�t t�rolja el a gratul�l� scene m?k�d�s�hez

        //�rt�kad�s a fent l�trehozott, Kinect-tel val� ir�ny�t�shoz sz�ks�ges gameObjeck-eknek a tag-ek seg�ts�g�vel
        left = GameObject.FindWithTag("HandLeft");
        right = GameObject.FindWithTag("HandRight");
    }

    void Update()
    {
        CheckInput(); //A felhaszn�l� �ltal gener�lt bevitel ellenorz�se
        CheckMovementDirection(); //Ellenorz�s, hogy a karakter j� ir�nyba n�z-e �ppen
        UpdateAnimations(); //A karakter anim�ci�inak folyamatos friss�t�s��rt felel a v�ltoz�k f�ggv�ny�ben
        CheckIfCanJump(); //Ellenorzi, hogy a karakter ugorhat-e
        CheckJump(); //A karakter ugr�s�nak v�grehajt�sa
    }

    private void FixedUpdate()
    {
        ApplyMovement(); //A karakter alapveto mozgat�s�nak v�grehajt�sa
        CheckSorroundings(); //A karakter-t k�r�lvevo elemek �rz�kel�se
    }

    private void CheckSorroundings() //A karakter-t k�r�lvevo elemek �rz�kel�se (pl., hogy a f�ld�n van-e a player)
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround); //Az isGrounded v�ltoz� elt�rolja, hogy a karakter �ppen a f�ld�n van-e.
                                                                                                     //Ehhez egy karakter k�r� rajzolt k�rt haszn�lja fel, hogy a karakter �rintkezik-e �ppen a whatIsGround v�ltoz�ban defini�lt f�lddel

        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y); //A falldetector a karakter-rel egy�tt mozog, de csak az X tengelyen. A karakter le�s�st �rz�keli
    }

    private void CheckInput() //A felhaszn�l� �ltal gener�lt bevitel ellenorz�se
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal"); //Alap�rtelmezetten ezzel meghat�rozzuk, hogy az A �s D billentyuk lenyom�s�val a v�ltoz� -1, illetve +1 �rt�ket adjon vissza.
                                                                 //Ha szimpl�n a GetAxis nevu met�dust h�vn�nk meg, akkor 0-t�l folyamatosan n�vekedne, illetve cs�kkenne a v�ltoz� �rt�ke,
                                                                 //am�g a +1-et vagy a -1-et el nem �ri, ahogy lenyomva tartjuk az A vagy D billentyut.
        if (Input.GetButtonDown("Jump")) //Ha a felhaszn�l� lenyomja az ugr�s billenty?j�t, akkor att�l f�gg?en, hogy �ppen a f�ld�n van-e, �s van ugr�sa, tud ugrani egyet
        {
            if (isGrounded || (amountOfJumpsLeft > 0))
            {
                NormalJump();
            }
            else //Ellenkezo esetben a jumptimer alap�rt�kre �ll�t�sa
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }

        if (!canMove) //Ha a karakter �ppen nem k�pes mozogni, akkor, ha a turnTimer kisebb vagy egyenlo 0-val, akkor a karakter mozgat�s�ra �s megfordul�s�ra ad lehetos�get a felhaszn�l�nak
        {
            turnTimer -= Time.deltaTime; //turnTimer cs�kkent�se

            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Input.GetButton("Jump")) //Ha a felhaszn�l� hamar felengedi az ugr�s gombot, akkor a karakter kisebbet fog ugrani, ha pedig hosszan nyomja meg, akkor tud a maxim�lis ugr�si erovel ugrani egyet a karakterrel. Ha a space-t lenyomja a felhaszn�l�, akkor false �rt�ket ad vissza, ha elengedi, akkor pedig true �rt�ket
        {
            checkJumpMultiplier = false;
            playerBody.velocity = new Vector2(playerBody.velocity.x, playerBody.velocity.y * variableJumpHeightMultiplier);
        }

        #region CheckInput_Kinect
        left = GameObject.FindWithTag("HandLeft"); //A left v�ltoz� megkapja a HandLeft tag-gel ell�ltott gameobject-et
        right = GameObject.FindWithTag("HandRight"); //A right v�ltoz� megkapja a HandRight tag-gel ell�ltott gameobject-et
        ///<summary>
        ///Mozg�s balra kinect seg�ts�g�vel
        ///A bal k�z poz�ci�j�t viszony�tjuk a referencia ponthoz.
        ///Ha a k�z ehhez k�pest balra �s 1 blokkal feljebb helyezkedik el, akkor a karakter balra megy, ha 3 blokkal, akkor balra megy �s ugrik.
        ///Az al�bbi k�d csak akkor fut le, ha van csatlakoztatva KINECT szenzor, �s az �rz�kelte a felhaszn�l�t.
        ///</summary>
        if (left!=null)
        {
            if (left.transform.position.x < handpositionchecker.transform.position.x && left.transform.position.y > handpositionchecker.transform.position.y) //Ha az �rz�kelt bal k�z az X koordin�t�ja a referencia ponthoz k�pest balra van, �s az Y koodin�t�ja a referencia ponthoz k�pest feljebb van
            {
                if (isGrounded && left.transform.position.y > 1f) //Ha a karakter a f�ld�n a van, �s a k�z a referencia ponthoz k�pest 1 egys�ggel van feljebb,
                {
                    MoveType = "kinect_move_left_and_jump"; //Akkor a MoveType erre lesz be�ll�tva (a karakter balra, ugr�lva fog haladni)
                }
                else //Ha a k�z a referencia pont �s az Y=1 k�z�tt van, akkor a karakter az adott ir�nyba mozog (nem ugrik)
                {
                    MoveType = "kinect_move_left";
                }
            }
            else //Az�rt sz�ks�ges, hogy az adott mozg�s t�pus ne a v�gtelens�gig �lljon fent. Ez�rt ebben a szakaszban alap�rt�kre �ll�tjuk vissza a MoveType-ot
            {
                MoveType = " ";
            }
            if (right.transform.position.x > handpositionchecker.transform.position.x && right.transform.position.y > handpositionchecker.transform.position.y) //Ha az �rz�kelt jobb k�z az x tengelyen a referencia ponthoz k�pest jobbra van �s az y koodin�t�ja a referencia ponthoz k�pest feljebb van
            {
                if (isGrounded && right.transform.position.y > 1f) //Ha a karakter a f�ld�n a van, �s a k�z a referencia ponthoz k�pest 1 egys�ggel van felemelve
                {
                    MoveType = "kinect_move_right_and_jump"; //Akkor a MoveType erre lesz be�ll�tva (a karakter jobbra, ugr�lva fog haladni)
                }
                else //Ha a k�z a referncia pont �s az Y=1 k�z�tt van, akkor a karakter az adott ir�nyba mozog (nem ugrik)
                {
                    MoveType = "kinect_move_right";
                }
            }
        }
        #endregion CheckInput_Kinect
    }

    private void CheckMovementDirection() //Ellenorz�s, hogy a karakter j� ir�nyba n�z-e �ppen
    {
        if (isFacingRight && movementInputDirection < 0) //Ha a karakter �ppen a megfelel? ir�nyba n�z, viszont a felhaszn�l� ellenkez? ir�nyba pr�b�lja meg ir�ny�tani a karaktert, akkor...
        {
            Flip(); //A karakter megford�t�sa
        }
        else if (!isFacingRight && movementInputDirection > 0) //Ha a felhaszn�l� a karakter aktu�lis n�z�si ir�ny�ba pr�b�lja meg mozgatni a karaktert, de a karakter rossz ir�nyba n�z, akkor...
        {
            Flip(); //A karakter megford�t�sa
        }

        if (Mathf.Abs(playerBody.velocity.x) >= 0.01f) //Ha a karakter X tengelyen t�rt�n? mozgat�s�nak abszol�t �rt�ke (Az ellenkez? ir�ny negat�v lenne) nagyobb, mint 0.01, akkor a karakter �ppen s�t�l
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void UpdateAnimations() //A karakter anim�ci�inak folyamatos friss�t�s��rt felel a v�ltoz�k f�ggv�ny�ben
    {
        playerAnimator.SetFloat("Speed", Mathf.Abs(playerBody.velocity.x)); //Ez a v�ltoz� az anim�ci� sebess�g�t adja meg
        playerAnimator.SetBool("OnGround", isGrounded); //Ez a v�ltoz� megn�zi, hogy a f�ld�n van-e a karakter (Az�rt kell, hogy f�ld�n �llva ne legyen ugr�s anim�ci�)
    }

    private void CheckIfCanJump() //Ellenorzi, hogy a karakter ugorhat-e
    {
        if (isGrounded && playerBody.velocity.y <= 0.01f) //Ha a karakter �ppen a f�ld�n van, �s nem halad az Y tengelyen lefel�, akkor a lehets�ges ugr�sok sz�m�nak alap�rt�kre �ll�t�sa
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if (amountOfJumpsLeft > 0)
        {
            canNormalJump = true;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canNormalJump = false;
        }

        if (amountOfJumpsLeft == amountOfJumps)
        {
            if (isGrounded) canNormalJump = true;
            if (!isGrounded) canNormalJump = false;
        }
    }

    private void CheckJump() //A karakter ugr�s�nak v�grehajt�sa
    {
        if (jumpTimer > 0) //Ha a jumpTimer nagyobb, mint 0, �s a karakter �ppen a f�ld�n van, akkor a karakter tud egy norm�l ugr�st csin�lni
        {
            //NormalJump
            if (isGrounded)
            {
                NormalJump();
            }
        }

        if (isAttemptingToJump) //Ha a felhaszn�l� a bevitele alapj�n pr�b�lkozott ugrani a karakterrel, de 
        {
            jumpTimer -= Time.deltaTime; //Stopper/Sz�ml�l� idej�nek cs�kkent�se
        }
    }

    private void ApplyMovement() //A karakter alapveto mozgat�s�nak v�grehajt�sa
    {
        if (!isGrounded && movementInputDirection == 0) //Ha a karakter �ppen a levegoben van, �s kapott bevitelt a felhaszn�l�t�l, akkor l�gellen�ll�s l�p fel a karakterrel szemben, ami fokozatosan cs�kkenti a sebess�g�t
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x * airDragMultiplier, playerBody.velocity.y);
        }
        else if (canMove) //A player csak akkor halad oldalir�nyba (az X tengely szerint), ha �ppen a f�ld�n van
        {
            playerBody.velocity = new Vector2(movementSpeed * movementInputDirection, playerBody.velocity.y);  //A player a defini�lt mozg�si sebess�ggel a defini�lt mozg�si ir�nyba kezd el haladni az X tengelyen,
                                                                                                               //az Y tengelyen pedig megtartja az eddigi sebess�g�t
        }

        #region Kinect_ApplyMovement
        if (MoveType == "kinect_move_left")
        {
            playerBody.velocity = new Vector2(movementSpeed * -1, playerBody.velocity.y);
            transform.localScale = new Vector2(-1f, 1f);
        }
        if (MoveType == "kinect_move_left_and_jump")
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, jumpForce);
        }
        if (MoveType == "kinect_move_right")
        {
            playerBody.velocity = new Vector2(movementSpeed, playerBody.velocity.y);
            transform.localScale = new Vector2(1f, 1f);
        }
        if (MoveType == "kinect_move_right_and_jump")
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, jumpForce);
        }
        # endregion Kinect_ApplyMovement
    }

    private void NormalJump() //A norm�l ugr�s v�grehajt�sa
    {
        if (canNormalJump) //Ha a karakter �ppen k�pes norm�l t�pus� ugr�sra
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, jumpForce); //A player y tengelyen t�rt�no gyorsul�sa a megadott ugr�si ero lesz, az x tengelyen pedig megmarad az eddigi sebess�ge
            amountOfJumpsLeft--; //A marad�k ugr�sok sz�m�nak cs�kkent�se 1-gyel
            jumpTimer = 0;
            isAttemptingToJump = false; //Az ugr�s v�grehajt�sa ut�n a karakter m�r nem pr�b�l ugrani
            checkJumpMultiplier = true;
        }
    }

    private void Flip() //A karakter megford�t�sa, ha a rossz ir�nyba n�z �ppen
    {
        if (canFlip) //A karakter megford�t�sa csak akkor, hogyha �ppen nem falon cs�szik
        {
            facingDirection *= -1; //A v�ltoz� jelenlegi �rt�k�nek ellenkez?re �ll�t�sa, amely sz�m t�pus� �rt�kk�nt t�rolja el, hogy a karakter �ppen a megfelel? ir�nyba n�z-e
            isFacingRight = !isFacingRight; //A v�ltoz� �rt�k�nek ellenkez?re �ll�t�sa, amely logikai t�pus� �rt�kk�nt t�rolja el, hogy a karakter �ppen a megfelel? ir�nyba n�z-e
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y); //A karakter megford�t�sa az Y tengely szerint 180 fokkal (Az X �s Z tengely szerint v�ltozatlan marad)
            //transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void OnDrawGizmos() //A karakter k�r� rajzolt k�r l�trehoz�sa a GroundCheck nevu gameobject seg�ts�g�vel, ami a talajt/f�ldet �rz�keli
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); //A karakter k�r� rajzol egy k�rt a megadott k�z�ppont(groundCheck.position) �s sug�r(groundCheckRadius) f�ggv�ny�ben, ami a karakter k�rnyezet�t �rz�keli
    }

    public void RespawnCheckPoint() //A Visual Studio az�rt �r 0 hivatkoz�st (reference-t), mert ez a met�dus csak a Unity Editor-on bel�l van felhaszn�lva, m�ghozz� a pausemenu "Restart Checkpoint" gombj�ra
    {
        //lifeText.text = lifeCounter.ToString(); //A k�pernyo bal felso sark�ba l�vo sz�ml�l� �rt�k�nek aktu�lis �rt�kre �ll�t�sa
        transform.position = respawnPoint; //A karakter poz�ci�j�nak a legut�bbi checkpoint-ra �ll�t�sa
    }

    private void Die() //A karakter hal�lakor megh�v�d� met�dus
    {
        script.stopwatchActive = false; //A k�pernyo felso r�sz�n, k�z�pen l�that� stopper�ra meg�ll�t�sa (ido sz�mol�s�nak meg�ll�t�sa)
        dieSound.Play(); //A karakter hal�l�t jelzo hangeffekt lej�tsz�sa
        lifeCounter--; //A karakter fennmarad� �letei sz�m�na cs�kkent�se 1-gyel

        if (lifeCounter < 0) //Ha a karakter hal�la ut�n a fennmarad� �letek elfogytak (kevesebb, mint 0 maradt), akkor �jraind�tjuk a scene-t
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else //Ellenkezo esetben friss�tj�k az �letek sz�m�t, majd �jra�lesztj�k a karaktert
        {
            lifeText.text = lifeCounter.ToString(); //A k�pernyo bal felso sark�ba l�vo sz�ml�l� �rt�k�nek aktu�lis �rt�kre �ll�t�sa
            transform.position = respawnPoint; //A karakter poz�ci�j�nak a legut�bbi checkpoint-ra �ll�t�sa
        }
        StartCoroutine(CountdownToStart()); //Megh�vja a CountdownToStart IEnumerator t�pus� met�dust, amely a karakter �jra�led�s�t �s a visszasz�ml�l�st vez�rli
    }

    IEnumerator CountdownToStart() //A karakter hal�la ut�n a karakter azonnali mozg�s�t akad�lyozza meg. Meg�ll�tja az id?t �s elind�t egy visszasz�ml�l�t, aminek lej�rta ut�n folytat�dik a j�t�k, �s a felhaszn�l� ism�t k�pes ir�ny�tni a karaktert
    {
        playerBody.bodyType = RigidbodyType2D.Static; //A karakter statikuss� alak�t�sa, hogy ne tudjon r�gt�n mozogni az �jra�led�se ut�n (hib�k elker�l�se �rdek�ben)
        countdownDisplay.gameObject.SetActive(true); //A visszasz�ml�l� gameobject l�that�v� t�tele
        while (countdownTime > 0) //Am�g tart a visszasz�ml�s, addig
        {
            countdownDisplay.text = countdownTime.ToString(); //A visszasz�ml�l� sz�veg�nek az aktu�lis m�sodpercre friss�t�se

            yield return new WaitForSeconds(1f); //V�rakoz�s 1 m�sodperc erej�ig

            countdownTime--; //A visszasz�ml�l�si ido cs�kkent�se 1-gyel
        }

        countdownDisplay.text = "GO!"; //A visszasz�ml�s ut�n a GO felirat megjelen�t�se

        yield return new WaitForSeconds(1f); //V�rakoz�s 1 m�sodperc erej�ig

        countdownDisplay.gameObject.SetActive(false); //A visszasz�ml�l� gameobject l�thatatlann� t�tele
        countdownTime = 3;
        playerBody.bodyType = RigidbodyType2D.Dynamic; //A karakter mozgat�s�nak lets�gess� t�tele azzal, hogy a karakter t�pus�t vissza�ll�tjuk dinamikusra
        script.stopwatchActive = true; //A k�pernyo felso r�sz�n, k�z�pen l�that� stopper�ra (nem �jra)ind�t�sa (ido sz�mol�s�nak folytat�sa)
    }

    /// <summary>
    /// Az al�bbi met�dusok h�v�dnak meg, amikor a karakter valamilyen gameobject-tel �rintkezik.
    /// Itt foglalnak helyet az al�bbiak: FallDetector, Checkpoint, Finish, Trap, TrapTile, Camera
    /// </summary>
    /// <param name="collision">Ez a v�ltoz� az az objektum, amely kiv�lt valamilyen t�pus� trigger-t, amikor �rintkezik vele a karakter</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //------------FallDetector------------
        if (collision.tag == "FallDetector" || collision.tag == "trap") //Ha egy "FallDetector" vagy "trap" tag-gel ell�tott objektummal �rintkezik a karakter, akkor meghal, majd �jra�led
        {
            Die();
            script.score = script.score - 50;
        }
        //-------------Checkpoint-------------
        if (collision.tag == "Checkpoint") //Ha egy Checkpoint-hoz �r hozz� a karakter, akkor a respawnpoint v�ltoz� azt a kordin�t�t t�rolja el, majd a checkpoint gameobject-et megsemmis�ti
        {
            respawnPoint = transform.position;
            checkpointSound.Play();
            Destroy(collision.gameObject);
        }
        //---------------Finish---------------
        if (collision.gameObject.CompareTag("finish")) //Ha a p�lya v�g�n a finish-hez �r a karakter, akkor a k�vetkez� p�lya(scene) ker�l bet�lt�sre
        {
            script.score = script.score - script.timeScore; //Az eltelt ido miatt pontlevon�s az �sszpontsz�mb�l
            finalScore = script.score; //Az �sszpontsz�m meghat�roz�sa
            if (nextSceneLoad > PlayerPrefs.GetInt("levelAt")) //A k�vetkezo p�lya felold�sa a felhaszn�l� sz�m�ra
            {
                PlayerPrefs.SetInt("levelAt", nextSceneLoad);
            }
            SceneManager.LoadScene("Congrats"); //A gratul�l� scene elind�t�sa (�sszpontsz�m, stb.)
        }
        //--------------TrapTile--------------
        if (collision.tag == "TrapTile")
        {
            Traps.SetActive(true);
            TrapTiles.SetActive(false);
        }
        //---------------Camera---------------
        if (collision.tag == "zoomout") //A kamer�t zoom-olja ki, hogy nagyobb teret l�sson a felhaszn�l�
        {
            cam.orthographicSize = 7;
        }
        if (collision.tag == "zoomin") //A kamera zoom-j�t vissza�ll�tja az alap�rtelmezett �rt�kre
        {
            cam.orthographicSize = normalCam;
        }
    }

    #region Movingplatfrom_muk�d�s�hez sz�ks�ges met�dusok
    private void OnCollisionEnter2D(Collision2D collision) //A movingplatform-ok m?k�d�s�hez sz�ks�ges met�dus. Megh�v�dik, amikor a karakter egy movingplatfrom-ra �ll
    {
        if (collision.gameObject.tag == "movingplatform") //Ha egy movingplatformhoz �r a karakter, akkor a karakter a sz�lo gameobject-je a platform lesz, �gy mozog vele egy�tt
        {
            if (collision.transform.position.y < transform.position.y - 0.5F) //Ha a platform felett van a karakter, akkor be�ll�tja a player sz�lo gameobject-j�nek
            {
                transform.parent = collision.gameObject.transform;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) //A movingplatform-ok m?k�d�s�hez sz�ks�ges met�dus. Megh�v�dik, amikor a karakter lesz�ll egy movingplatform-r�l
    {
        if (collision.gameObject.tag == "movingplatform") //Ha a movingplatformr�l leugrik a karakter, akkor vissza�ll�tja a player sz�lo gameobject-j�t null �rt�kre
        {
            transform.parent = null;
        }
    }
    #endregion Movingplatfrom_muk�d�s�hez sz�ks�ges met�dusok
}