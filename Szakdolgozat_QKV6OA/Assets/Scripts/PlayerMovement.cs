using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Ez itt a PlayerController-ünk, itt irányítjuk/határozzuk meg a karaker mozgását, külöböz? tulajdonságait.
/// Itt található még egy groundCheck gameobject-re mutató referencia is, amely megnézi, hogy a karakter a földön van-e ezt egy groundCheckRadius nev? sugaron belül teszi. Itt van még egy LayerMask is, ami azért kell, hogy csak azokat
/// nézze amelyeknek a Ground Layer-re van állítva. Az isTouchningGround változó azt nézi meg, hogy a karakter éppen hozzáér-e a földhöz.
/// A respawnpoint tárolja azokat a kordinátákat, ahova a karakter kerül miután meghalt.
/// A fallDetector érzékeli, ha a karakter kiesik a pályáról. A lifeCounter az éppen elérhetõ életet tárolja el.
/// A handpositionchecker gameobject-re mutató referencia egy statikus object a pályán, amihez képest majd a Kinect által érzékelt kezeket viszonyítjuk, amelyek a left és right GameObjectben lesznek eltárolva.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    //variables section 1
    private string MoveType; //A Kinect irányításához szükséges változó, amely a karakter pontos mozgásának típusát tárolja el (pl.: kinect_move_left_and_jump)

    private int lifeCounter; //Definiálja a karakter fennmaradó életeinek számát

    private float normalCam; //A felhasználó számára látható teret mutató kamera látószöge/mérete

    private Rigidbody2D playerBody; //A karaktert, a Player gameobject Rigidbody 2D komponensére hivatkozva lehetséges mozgásra bírni, ezért hivatkozunk rá a kódban
    private Animator playerAnimator; //A karaktert, a Player gameobject Animator komponensére hivatkozva lehetséges meganimálni, ezért hivatkozunk rá a kódban
    private Vector3 respawnPoint; //A karakter újraéledésének helyét tárolja el

    public static int finalScore; //Az adott pálya teljesítésekor megjelen? összpontszám
    public static int lastSceneNumber; //A legutóbb játszott scene buildszámát tárolja el (Szükséges a Congrats scene-hez és script-hez)

    public int countdownTime; //A karakter újraéledésekor eltelo visszaszámláló idotartamát tárolja el, mielott a felhasználó újra tudja irányítani a karaktert

    public GameObject fallDetector; //Érzékeli, ha a karakter leesik a pályáról
    public GameObject TrapTiles; //A 3. szinten lévo csapdák feletti föld elemek szülo gameobject-jére mutató referenciát tárolja el
    public GameObject Traps; //A 3. szinten lévo csapdák szülo gameobject-jére mutató referenciát tárolja el
    public GameObject handpositionchecker; //A Kinect irányításához szükséges változó, amely a kezek aktuális pozícióját definiálja
    public GameObject left; //A Kinect irányításához szükséges változó, amely a bal kéz aktuális pozícióját határozza meg
    public GameObject right; //A Kinect irányításához szükséges változó, amely a jobb kéz aktuális pozícióját határozza meg

    public Text lifeText; //A karakter hátralévo életeinek számát jeleníti meg
    public Text countdownDisplay; //A karakter újraéledésekor megjeleno visszaszámlálót jeleníti meg
    public Camera cam; //A felhasználó számára látható teret mutató kamera
    public Timer script; //A visszaszámláló kódjának referenciája
    public AudioSource checkpointSound; //A checkpoint-ok felvételekor lejátszódó hangeffekt referenciája
    public AudioSource dieSound; //A karakter halálakor lejátszódó hangeffekt referenciája

    //variables section 2
    private float movementInputDirection; //Ebben a változóban tároljuk el, hogy a karakter éppen melyik irányba mozog (-1, 0, vagy +1 értéket ad vissza)
    private float jumpTimer;
    private float turnTimer;

    private int amountOfJumpsLeft; //A maradék ugrások számát tárolja el
    private int facingDirection = 1; //A karakter aktuális irányát tárolja el -1 vagy +1 formában. Kezdetben azért 1 mert a pályák elindulásakor a karakter mindig jobbra néz

    private bool isFacingRight = true; //Ebben a változóban tároljuk el, hogy a karakter éppen jó irányba néz-e. Amikor újraéled, akkor mindig jó irányba néz, ezért alapértelmezetten true értéku
    private bool isWalking; //Ez a változó eltárolja, hogy a karakter éppen a földön sétál vagy nem
    private bool isGrounded; //Ez a változó eltárolja, hogy a karakter éppen a földön van-e
    private bool canNormalJump; //Eltárolja, hogy a karakter éppen ugorhat-e normál módon vagy sem
    private bool isAttemptingToJump; //Eltárolja, hogy a karakter éppen ugrani próbál-e (A felhasználó bevitele alapján)
    private bool checkJumpMultiplier;
    private bool canMove; //Eltárolja, hogy a karakter éppen mozoghat-e
    private bool canFlip; //Eltárolja, hogy a karakter éppen megfordulhat-e

    public int amountOfJumps = 1; //Definiálja, hogy a karakter hányszor képes ugrani addig, amíg újra a földhöz nem ér

    public float movementSpeed = 10.0f; //A karakter mozgási sebességét határozza meg
    public float jumpForce = 16.0f; //A karakter ugrásakor a karakter Y tengely szerinti gyorsulásának értéke, tehát az ugrásának ereje
    public float groundCheckRadius; //A karakter körüli sugár, ami érzékeli, hogy a karakter a földön van-e
    public float movementForceInAir; //A karakterre ható ero nagysága, ha a levegoben irányítja a felhasználó
    public float airDragMultiplier = 0.95f; //A karakterre ható légellenállás ereje, amikor a levegoben kap bevitelt a felhasználótól
    public float variableJumpHeightMultiplier = 0.5f; //Arra szolgál, hogy figyelembe vegye a felhasználó által lenyomott ugrási billentyu lenyomási idejét, ezáltal befolyásolva, hogy milyen magasra ugrik fel a karakter
    public float jumpTimerSet = 0.15f;

    public Transform groundCheck; //Ez változó hivatkozik a GroundCheck gameobject-re, amivel ellenorizzük, hogy a karakter a földön van-e éppen

    public LayerMask whatIsGround; //Ez a változó definiálja a talajt/földet

    public int nextSceneLoad; //A következo pálya indexét tárolja el

    // Start is called before the first frame update
    /// <summary>
    /// A játék inditásakor a player változó megkapja a player objektumot, a playerAnimation pedig az animator komponenst. A respawnPoint értéke a player alap pozíciójára áll be.
    /// Az kezdõéleterõ 3.
    /// A normalCam pedig a Camera kezdõ zoom értéket tárolja.
    /// </summary>
    void Start()
    {
        //Apply graphics settings
        QualitySettings.SetQualityLevel(SettingsMenu.qualityIndex_Out);

        //Gameobject Komponensek referenciáinak eltárolása
        playerBody = GetComponent<Rigidbody2D>(); //Ezzel tárolunk el egy referenciát a player Rigidbody 2D komponensére, illetve itt kapja meg a rigidbody2D változó a Player gameobject elso olyan komponensét,
                                                  //aminek Rigidbody 2D a típusa, a kód elindulásakor
        playerAnimator = GetComponent<Animator>(); //Ezzel tárolunk el egy referenciát a player Animator komponensére, illetve itt kapja meg az animator változó a Player gameobject elso olyan komponensét,
                                                   //aminek Animator a típusa, a kód elindulásakor

        //Változók kezdeti értékeinek beállítása
        amountOfJumpsLeft = amountOfJumps; //A maradék ugrások számának alapértékre állítása
        respawnPoint = transform.position; //A karakter alapértelmezett újraéledésének pozíciója a kezd?pozíció
        lifeCounter = 3; //A karakter alapértelmezett életeinek számát határozza meg
        normalCam = cam.orthographicSize; //A kód indulásakor a kamera fókuszának alapértelmezettre állítása
        nextSceneLoad = SceneManager.GetActiveScene().buildIndex + 1; //A következo pálya indexét határozza meg
        lastSceneNumber = SceneManager.GetActiveScene().buildIndex; //A jelenlegi pálya indexét tárolja el a gratuláló scene m?ködéséhez

        //Értékadás a fent létrehozott, Kinect-tel való irányításhoz szükséges gameObjeck-eknek a tag-ek segítségével
        left = GameObject.FindWithTag("HandLeft");
        right = GameObject.FindWithTag("HandRight");
    }

    void Update()
    {
        CheckInput(); //A felhasználó által generált bevitel ellenorzése
        CheckMovementDirection(); //Ellenorzés, hogy a karakter jó irányba néz-e éppen
        UpdateAnimations(); //A karakter animációinak folyamatos frissítéséért felel a változók függvényében
        CheckIfCanJump(); //Ellenorzi, hogy a karakter ugorhat-e
        CheckJump(); //A karakter ugrásának végrehajtása
    }

    private void FixedUpdate()
    {
        ApplyMovement(); //A karakter alapveto mozgatásának végrehajtása
        CheckSorroundings(); //A karakter-t körülvevo elemek érzékelése
    }

    private void CheckSorroundings() //A karakter-t körülvevo elemek érzékelése (pl., hogy a földön van-e a player)
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround); //Az isGrounded változó eltárolja, hogy a karakter éppen a földön van-e.
                                                                                                     //Ehhez egy karakter köré rajzolt kört használja fel, hogy a karakter érintkezik-e éppen a whatIsGround változóban definiált földdel

        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y); //A falldetector a karakter-rel együtt mozog, de csak az X tengelyen. A karakter leésést érzékeli
    }

    private void CheckInput() //A felhasználó által generált bevitel ellenorzése
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal"); //Alapértelmezetten ezzel meghatározzuk, hogy az A és D billentyuk lenyomásával a változó -1, illetve +1 értéket adjon vissza.
                                                                 //Ha szimplán a GetAxis nevu metódust hívnánk meg, akkor 0-tól folyamatosan növekedne, illetve csökkenne a változó értéke,
                                                                 //amíg a +1-et vagy a -1-et el nem éri, ahogy lenyomva tartjuk az A vagy D billentyut.
        if (Input.GetButtonDown("Jump")) //Ha a felhasználó lenyomja az ugrás billenty?jét, akkor attól függ?en, hogy éppen a földön van-e, és van ugrása, tud ugrani egyet
        {
            if (isGrounded || (amountOfJumpsLeft > 0))
            {
                NormalJump();
            }
            else //Ellenkezo esetben a jumptimer alapértékre állítása
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }

        if (!canMove) //Ha a karakter éppen nem képes mozogni, akkor, ha a turnTimer kisebb vagy egyenlo 0-val, akkor a karakter mozgatására és megfordulására ad lehetoséget a felhasználónak
        {
            turnTimer -= Time.deltaTime; //turnTimer csökkentése

            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Input.GetButton("Jump")) //Ha a felhasználó hamar felengedi az ugrás gombot, akkor a karakter kisebbet fog ugrani, ha pedig hosszan nyomja meg, akkor tud a maximális ugrási erovel ugrani egyet a karakterrel. Ha a space-t lenyomja a felhasználó, akkor false értéket ad vissza, ha elengedi, akkor pedig true értéket
        {
            checkJumpMultiplier = false;
            playerBody.velocity = new Vector2(playerBody.velocity.x, playerBody.velocity.y * variableJumpHeightMultiplier);
        }

        #region CheckInput_Kinect
        left = GameObject.FindWithTag("HandLeft"); //A left változó megkapja a HandLeft tag-gel elláltott gameobject-et
        right = GameObject.FindWithTag("HandRight"); //A right változó megkapja a HandRight tag-gel elláltott gameobject-et
        ///<summary>
        ///Mozgás balra kinect segítségével
        ///A bal kéz pozícióját viszonyítjuk a referencia ponthoz.
        ///Ha a kéz ehhez képest balra és 1 blokkal feljebb helyezkedik el, akkor a karakter balra megy, ha 3 blokkal, akkor balra megy és ugrik.
        ///Az alábbi kód csak akkor fut le, ha van csatlakoztatva KINECT szenzor, és az érzékelte a felhasználót.
        ///</summary>
        if (left!=null)
        {
            if (left.transform.position.x < handpositionchecker.transform.position.x && left.transform.position.y > handpositionchecker.transform.position.y) //Ha az érzékelt bal kéz az X koordinátája a referencia ponthoz képest balra van, és az Y koodinátája a referencia ponthoz képest feljebb van
            {
                if (isGrounded && left.transform.position.y > 1f) //Ha a karakter a földön a van, és a kéz a referencia ponthoz képest 1 egységgel van feljebb,
                {
                    MoveType = "kinect_move_left_and_jump"; //Akkor a MoveType erre lesz beállítva (a karakter balra, ugrálva fog haladni)
                }
                else //Ha a kéz a referencia pont és az Y=1 között van, akkor a karakter az adott irányba mozog (nem ugrik)
                {
                    MoveType = "kinect_move_left";
                }
            }
            else //Azért szükséges, hogy az adott mozgás típus ne a végtelenségig álljon fent. Ezért ebben a szakaszban alapértékre állítjuk vissza a MoveType-ot
            {
                MoveType = " ";
            }
            if (right.transform.position.x > handpositionchecker.transform.position.x && right.transform.position.y > handpositionchecker.transform.position.y) //Ha az érzékelt jobb kéz az x tengelyen a referencia ponthoz képest jobbra van és az y koodinátája a referencia ponthoz képest feljebb van
            {
                if (isGrounded && right.transform.position.y > 1f) //Ha a karakter a földön a van, és a kéz a referencia ponthoz képest 1 egységgel van felemelve
                {
                    MoveType = "kinect_move_right_and_jump"; //Akkor a MoveType erre lesz beállítva (a karakter jobbra, ugrálva fog haladni)
                }
                else //Ha a kéz a referncia pont és az Y=1 között van, akkor a karakter az adott irányba mozog (nem ugrik)
                {
                    MoveType = "kinect_move_right";
                }
            }
        }
        #endregion CheckInput_Kinect
    }

    private void CheckMovementDirection() //Ellenorzés, hogy a karakter jó irányba néz-e éppen
    {
        if (isFacingRight && movementInputDirection < 0) //Ha a karakter éppen a megfelel? irányba néz, viszont a felhasználó ellenkez? irányba próbálja meg irányítani a karaktert, akkor...
        {
            Flip(); //A karakter megfordítása
        }
        else if (!isFacingRight && movementInputDirection > 0) //Ha a felhasználó a karakter aktuális nézési irányába próbálja meg mozgatni a karaktert, de a karakter rossz irányba néz, akkor...
        {
            Flip(); //A karakter megfordítása
        }

        if (Mathf.Abs(playerBody.velocity.x) >= 0.01f) //Ha a karakter X tengelyen történ? mozgatásának abszolút értéke (Az ellenkez? irány negatív lenne) nagyobb, mint 0.01, akkor a karakter éppen sétál
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void UpdateAnimations() //A karakter animációinak folyamatos frissítéséért felel a változók függvényében
    {
        playerAnimator.SetFloat("Speed", Mathf.Abs(playerBody.velocity.x)); //Ez a változó az animáció sebességét adja meg
        playerAnimator.SetBool("OnGround", isGrounded); //Ez a változó megnézi, hogy a földön van-e a karakter (Azért kell, hogy földön állva ne legyen ugrás animáció)
    }

    private void CheckIfCanJump() //Ellenorzi, hogy a karakter ugorhat-e
    {
        if (isGrounded && playerBody.velocity.y <= 0.01f) //Ha a karakter éppen a földön van, és nem halad az Y tengelyen lefelé, akkor a lehetséges ugrások számának alapértékre állítása
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

    private void CheckJump() //A karakter ugrásának végrehajtása
    {
        if (jumpTimer > 0) //Ha a jumpTimer nagyobb, mint 0, és a karakter éppen a földön van, akkor a karakter tud egy normál ugrást csinálni
        {
            //NormalJump
            if (isGrounded)
            {
                NormalJump();
            }
        }

        if (isAttemptingToJump) //Ha a felhasználó a bevitele alapján próbálkozott ugrani a karakterrel, de 
        {
            jumpTimer -= Time.deltaTime; //Stopper/Számláló idejének csökkentése
        }
    }

    private void ApplyMovement() //A karakter alapveto mozgatásának végrehajtása
    {
        if (!isGrounded && movementInputDirection == 0) //Ha a karakter éppen a levegoben van, és kapott bevitelt a felhasználótól, akkor légellenállás lép fel a karakterrel szemben, ami fokozatosan csökkenti a sebességét
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x * airDragMultiplier, playerBody.velocity.y);
        }
        else if (canMove) //A player csak akkor halad oldalirányba (az X tengely szerint), ha éppen a földön van
        {
            playerBody.velocity = new Vector2(movementSpeed * movementInputDirection, playerBody.velocity.y);  //A player a definiált mozgási sebességgel a definiált mozgási irányba kezd el haladni az X tengelyen,
                                                                                                               //az Y tengelyen pedig megtartja az eddigi sebességét
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

    private void NormalJump() //A normál ugrás végrehajtása
    {
        if (canNormalJump) //Ha a karakter éppen képes normál típusú ugrásra
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, jumpForce); //A player y tengelyen történo gyorsulása a megadott ugrási ero lesz, az x tengelyen pedig megmarad az eddigi sebessége
            amountOfJumpsLeft--; //A maradék ugrások számának csökkentése 1-gyel
            jumpTimer = 0;
            isAttemptingToJump = false; //Az ugrás végrehajtása után a karakter már nem próbál ugrani
            checkJumpMultiplier = true;
        }
    }

    private void Flip() //A karakter megfordítása, ha a rossz irányba néz éppen
    {
        if (canFlip) //A karakter megfordítása csak akkor, hogyha éppen nem falon csúszik
        {
            facingDirection *= -1; //A változó jelenlegi értékének ellenkez?re állítása, amely szám típusú értékként tárolja el, hogy a karakter éppen a megfelel? irányba néz-e
            isFacingRight = !isFacingRight; //A változó értékének ellenkez?re állítása, amely logikai típusú értékként tárolja el, hogy a karakter éppen a megfelel? irányba néz-e
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y); //A karakter megfordítása az Y tengely szerint 180 fokkal (Az X és Z tengely szerint változatlan marad)
            //transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void OnDrawGizmos() //A karakter köré rajzolt kör létrehozása a GroundCheck nevu gameobject segítségével, ami a talajt/földet érzékeli
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); //A karakter köré rajzol egy kört a megadott középpont(groundCheck.position) és sugár(groundCheckRadius) függvényében, ami a karakter környezetét érzékeli
    }

    public void RespawnCheckPoint() //A Visual Studio azért ír 0 hivatkozást (reference-t), mert ez a metódus csak a Unity Editor-on belül van felhasználva, méghozzá a pausemenu "Restart Checkpoint" gombjára
    {
        //lifeText.text = lifeCounter.ToString(); //A képernyo bal felso sarkába lévo számláló értékének aktuális értékre állítása
        transform.position = respawnPoint; //A karakter pozíciójának a legutóbbi checkpoint-ra állítása
    }

    private void Die() //A karakter halálakor meghívódó metódus
    {
        script.stopwatchActive = false; //A képernyo felso részén, középen látható stopperóra megállítása (ido számolásának megállítása)
        dieSound.Play(); //A karakter halálát jelzo hangeffekt lejátszása
        lifeCounter--; //A karakter fennmaradó életei számána csökkentése 1-gyel

        if (lifeCounter < 0) //Ha a karakter halála után a fennmaradó életek elfogytak (kevesebb, mint 0 maradt), akkor újraindítjuk a scene-t
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else //Ellenkezo esetben frissítjük az életek számát, majd újraélesztjük a karaktert
        {
            lifeText.text = lifeCounter.ToString(); //A képernyo bal felso sarkába lévo számláló értékének aktuális értékre állítása
            transform.position = respawnPoint; //A karakter pozíciójának a legutóbbi checkpoint-ra állítása
        }
        StartCoroutine(CountdownToStart()); //Meghívja a CountdownToStart IEnumerator típusú metódust, amely a karakter újraéledését és a visszaszámlálást vezérli
    }

    IEnumerator CountdownToStart() //A karakter halála után a karakter azonnali mozgását akadályozza meg. Megállítja az id?t és elindít egy visszaszámlálót, aminek lejárta után folytatódik a játék, és a felhasználó ismét képes irányítni a karaktert
    {
        playerBody.bodyType = RigidbodyType2D.Static; //A karakter statikussá alakítása, hogy ne tudjon rögtön mozogni az újraéledése után (hibák elkerülése érdekében)
        countdownDisplay.gameObject.SetActive(true); //A visszaszámláló gameobject láthatóvá tétele
        while (countdownTime > 0) //Amíg tart a visszaszámlás, addig
        {
            countdownDisplay.text = countdownTime.ToString(); //A visszaszámláló szövegének az aktuális másodpercre frissítése

            yield return new WaitForSeconds(1f); //Várakozás 1 másodperc erejéig

            countdownTime--; //A visszaszámlálási ido csökkentése 1-gyel
        }

        countdownDisplay.text = "GO!"; //A visszaszámlás után a GO felirat megjelenítése

        yield return new WaitForSeconds(1f); //Várakozás 1 másodperc erejéig

        countdownDisplay.gameObject.SetActive(false); //A visszaszámláló gameobject láthatatlanná tétele
        countdownTime = 3;
        playerBody.bodyType = RigidbodyType2D.Dynamic; //A karakter mozgatásának letségessé tétele azzal, hogy a karakter típusát visszaállítjuk dinamikusra
        script.stopwatchActive = true; //A képernyo felso részén, középen látható stopperóra (nem újra)indítása (ido számolásának folytatása)
    }

    /// <summary>
    /// Az alábbi metódusok hívódnak meg, amikor a karakter valamilyen gameobject-tel érintkezik.
    /// Itt foglalnak helyet az alábbiak: FallDetector, Checkpoint, Finish, Trap, TrapTile, Camera
    /// </summary>
    /// <param name="collision">Ez a változó az az objektum, amely kivált valamilyen típusú trigger-t, amikor érintkezik vele a karakter</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //------------FallDetector------------
        if (collision.tag == "FallDetector" || collision.tag == "trap") //Ha egy "FallDetector" vagy "trap" tag-gel ellátott objektummal érintkezik a karakter, akkor meghal, majd újraéled
        {
            Die();
            script.score = script.score - 50;
        }
        //-------------Checkpoint-------------
        if (collision.tag == "Checkpoint") //Ha egy Checkpoint-hoz ér hozzá a karakter, akkor a respawnpoint változó azt a kordinátát tárolja el, majd a checkpoint gameobject-et megsemmisíti
        {
            respawnPoint = transform.position;
            checkpointSound.Play();
            Destroy(collision.gameObject);
        }
        //---------------Finish---------------
        if (collision.gameObject.CompareTag("finish")) //Ha a pálya végén a finish-hez ér a karakter, akkor a következõ pálya(scene) kerül betöltésre
        {
            script.score = script.score - script.timeScore; //Az eltelt ido miatt pontlevonás az összpontszámból
            finalScore = script.score; //Az összpontszám meghatározása
            if (nextSceneLoad > PlayerPrefs.GetInt("levelAt")) //A következo pálya feloldása a felhasználó számára
            {
                PlayerPrefs.SetInt("levelAt", nextSceneLoad);
            }
            SceneManager.LoadScene("Congrats"); //A gratuláló scene elindítása (összpontszám, stb.)
        }
        //--------------TrapTile--------------
        if (collision.tag == "TrapTile")
        {
            Traps.SetActive(true);
            TrapTiles.SetActive(false);
        }
        //---------------Camera---------------
        if (collision.tag == "zoomout") //A kamerát zoom-olja ki, hogy nagyobb teret lásson a felhasználó
        {
            cam.orthographicSize = 7;
        }
        if (collision.tag == "zoomin") //A kamera zoom-ját visszaállítja az alapértelmezett értékre
        {
            cam.orthographicSize = normalCam;
        }
    }

    #region Movingplatfrom_muködéséhez szükséges metódusok
    private void OnCollisionEnter2D(Collision2D collision) //A movingplatform-ok m?ködéséhez szükséges metódus. Meghívódik, amikor a karakter egy movingplatfrom-ra áll
    {
        if (collision.gameObject.tag == "movingplatform") //Ha egy movingplatformhoz ér a karakter, akkor a karakter a szülo gameobject-je a platform lesz, így mozog vele együtt
        {
            if (collision.transform.position.y < transform.position.y - 0.5F) //Ha a platform felett van a karakter, akkor beállítja a player szülo gameobject-jének
            {
                transform.parent = collision.gameObject.transform;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) //A movingplatform-ok m?ködéséhez szükséges metódus. Meghívódik, amikor a karakter leszáll egy movingplatform-ról
    {
        if (collision.gameObject.tag == "movingplatform") //Ha a movingplatformról leugrik a karakter, akkor visszaállítja a player szülo gameobject-jét null értékre
        {
            transform.parent = null;
        }
    }
    #endregion Movingplatfrom_muködéséhez szükséges metódusok
}