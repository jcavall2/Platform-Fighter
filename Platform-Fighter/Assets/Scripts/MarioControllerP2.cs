/*
    All controls for Player 2's Mario
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MarioControllerP2 : MonoBehaviour
{
    //Accesses the rigidbody, what gives it collision
    private Rigidbody2D rb;
    //Accesses the sprite renderer, makes the sprites connect better so the animations are more smooth
    private SpriteRenderer sprite;
    //Accesses the animator to play all the animations
    private Animator anim;
    //Accesses the box collider to give this Mario a hitbox
    private BoxCollider2D hurtBox;

    //Gives access to the percent script
    private Percent playerPercent;

    //Array of images for each stock Mario has
    public Image[] stockCount;
    //The number of stocks the Mario has
    [SerializeField] private int stocks = 3;
    //A gameobject of the stage
    [SerializeField] private LayerMask stage;

    //The state to send to the animator so it knows which animation to play
    private enum characterState { idle, running, jumping, falling, jab1, jab2, jab3, dashAtk, FSmash, NAir, BAir, DAir, DSmash, DTilt, FAir, DownB, SideTilt, UpAir, UpB, UpSmash, UpTilt, SpFall, Hurt }
    //Array of hitboxes for each attack
    public BoxCollider2D[] attackHitboxes;
    //The direction Mario is moving on the X-axis, negative to the left and positive to the right
    private float dirX = 0f;
    //How fast Mario moves per frame of movement
    [SerializeField] private float movementSpeed = 7f;
    //How high Mario jumps per jump
    [SerializeField] private float jumpHeight = 12;
    //How scaled up Mario is to make him a better size
    private const float scale = 1.652458f;
    //True if Mario can double jump, false if he already double jumped and can't do it again
    private bool canDoubleJump = true;
    //True if Mario is in the animation of doing an attack or getting hit, false otherwise
    private bool inAnimation = false;
    //String of which attack Mario is currently using
    private string attack = "";
    //Timer for how long an attack animation takes
    private float attackTimer = 0f;
    //Timer for long you can wait between using jab1 to jab2 if the timer runs out you will just use jab1 instead
    [SerializeField] private float jab1Timer = 0f;
    //Timer for long you can wait between using jab2 to jab3 if the timer runs out you will just use jab2 instead
    [SerializeField] private float jab2Timer = 0f;
    //True if Mario is using an aerial attack, false otherwise
    private bool inAerial = false;
    //True if Mario used Up B attack, false otherwise
    private bool usedUpB = false;
    //True if Mario is in special fall, false otherwise
    private bool inSpecialFall = false;
    //Timer for when the hitbox for an attack should start
    private float hitBoxStart = 0f;
    //Timer for when the hitbox for an attack should end
    private float hitBoxEnd = 0f;
    //True if Mario has already hit the enemy with his attack
    private bool enemyHit = false;
    //How long Mario can't move for when he's hit
    private float attackStunTimer = 0f;
    //True if Mario was knocked off stage, false otherwise
    private bool isDead = false;
    //How long Mario is dead for before he respawns
    private float deathTimer;

    //The game object where the sound effects come from
    public AudioSource soundEffects;
    //The sound of Mario getting hit
    public AudioClip hitSound;
    //The sound of Mario getting knocked out
    public AudioClip knockout;

    /**/
    /*
    MarioControllerP2::Start()

    NAME
        MarioControllerP2::Start() - Start is called before the first frame update

    Synopsis
        void MarioControllerP2::Start();

    Description
        Gets the components that are attached to the Mario game object and sets them to the variables made already. Also turns Mario around since player 2's Mario has to face the other way at first

    RETURNS
        Returns nothing
    */
    /**/
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        hurtBox = GetComponent<BoxCollider2D>();
        playerPercent = GetComponent<Percent>();
        //Multiplies X value by -1 to turn Mario around at the beginning
        transform.localScale = new Vector3(-1 * scale, scale, scale);
    }

    /**/
    /*
    MarioControllerP2::Update()

    NAME
        MarioControllerP2::Update() - Update is called once per frame

    Synopsis
        void MarioControllerP2::Update();

    Description
        Controls everything that has to happen every frame. Checks if the user is trying to move or use an attack. Checks if Mario has been knocked off screen, respawns him if he was.

    RETURNS
        Returns nothing
    */
    /**/
    private void Update()
    {
        //If Mario is not dead
        if (!isDead)
        {
            //If Mario was attacked count down the attack stun timer before Mario can move again
            if (attackStunTimer > 0)
            {
                attackStunTimer -= Time.deltaTime;
            }

            //If Mario has not been attacked recently
            else
            {
                //If Mario is in an attack animation then can't use another attack
                if (inAnimation)
                {
                    //Checks if Mario landed his attack
                    CheckAttackHit(attack);
                }
                //If used Up B
                if (usedUpB)
                {
                    //Once Mario starts falling after Up B put into special fall and change the animation
                    if (rb.velocity.y < -.1f)
                    {
                        inSpecialFall = true;
                        //Puts Mario in the special fall animation
                        UpdateAnimation("SpFall");
                        usedUpB = false;
                    }
                }

                //If Mario is in special fall
                else if (inSpecialFall)
                {
                    //Mario only moves half as much horizontally when in special fall, this moves him to the left
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        dirX = -0.5f;
                    }

                    //Mario only moves half as much horizontally when in special fall, this moves him to the right
                    else if (Input.GetKey(KeyCode.RightArrow))
                    {
                        dirX = 0.5f;
                    }
                    //What actually makes Mario move with the direction being used before
                    rb.velocity = new Vector2(dirX * movementSpeed, rb.velocity.y);

                    //Resets the direction to 0 so Mario doesn't keep moving after you move first
                    dirX = 0;

                    //If Mario touches the ground end special fall
                    if (IsOnGround())
                    {
                        inSpecialFall = false;
                    }
                }

                //If not not special fall or Up B
                else
                {
                    //If in Aerial you can move while attacking
                    if (inAerial)
                    {
                        //Move to the left in the air
                        if (Input.GetKey(KeyCode.LeftArrow))
                        {
                            dirX = -1f;
                        }

                        //Move to the right in the air
                        else if (Input.GetKey(KeyCode.RightArrow))
                        {
                            dirX = 1f;
                        }
                        //What actually makes Mario move with the direction being used before
                        rb.velocity = new Vector2(dirX * movementSpeed, rb.velocity.y);

                        //Resets the direction to 0 so Mario doesn't keep moving after you move first
                        dirX = 0;

                        //If Mario touches the ground while using an aerial attack cancel the attack and reset him
                        if (IsOnGround())
                        {
                            inAerial = false;
                            attackTimer = 0f;
                        }
                    }

                    //if Mario is not doing an attack
                    if (attackTimer <= 0)
                    {
                        //Sets all these to false since he isn't attacking
                        inAerial = false;
                        inAnimation = false;
                        enemyHit = false;
                        attack = "";

                        //Makes Mario run to the left using the left arrow key
                        if (Input.GetKey(KeyCode.LeftArrow))
                        {
                            dirX = -1f;
                        }

                        //Makes Mario run to the right using the right arrow key
                        else if (Input.GetKey(KeyCode.RightArrow))
                        {
                            dirX = 1f;
                        }

                        //Checks what attack the user is trying to use
                        CheckAttack();

                        //Makes Mario move like normal unless he's using up B
                        if (!usedUpB)
                        {
                            rb.velocity = new Vector2(dirX * movementSpeed, rb.velocity.y);
                        }

                        //Mario gets double jump back once he touches the ground
                        if (IsOnGround())
                        {
                            canDoubleJump = true;
                        }

                        //If not in animation then Mario can jump
                        if (!inAnimation)
                        {
                            //pressing left square bracket makes Mario Jump
                            if (Input.GetKeyDown(KeyCode.LeftBracket))
                            {
                                //Makes Mario jump
                                if (IsOnGround())
                                {
                                    rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                                }
                                //Can only jump again if double jump has not been used
                                else if (canDoubleJump == true)
                                {
                                    rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                                    canDoubleJump = false;
                                }
                            }
                        }

                        //Calls function to update the animation based on what attack Mario used
                        UpdateAnimation(attack);

                        //Resets to 0 so Mario stops moving
                        dirX = 0f;
                    }

                    //If Mario is doing an attack
                    else
                    {
                        //Reduce the attack timer
                        attackTimer -= Time.deltaTime;
                    }
                    //Reduce the Jab timers
                    jab1Timer -= Time.deltaTime;
                    jab2Timer -= Time.deltaTime;
                }
            }
            //Checks if Mario has been knocked out
            KO();
        }

        //if Mario has been knocked out
        else
        {
            //Makes Mario able to move again
            if (deathTimer < 0)
            {
                isDead = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
                hurtBox.enabled = true;
            }
            //Puts Mario at the top of the screen invincible for 3 seconds before he can move again
            else if (deathTimer < 3)
            {
                //Makes Mario face to the left where he respawns
                transform.localScale = new Vector3(-1 * scale, scale, scale);
                //Puts him in his idle animation
                anim.SetInteger("state", 0);
                //Moves Mario to the top right part of the stage
                rb.transform.position = new Vector3(2, 5, 0);
                //Makes Mario static so he cannot move
                rb.bodyType = RigidbodyType2D.Static;
                //Turns off hurtbox so Mario cannot take damage
                hurtBox.enabled = false;
                //Updates Mario's percent so it goes back to 0%.
                playerPercent.UpdateHealth();
            }
            //reduces the death timer
            deathTimer -= Time.deltaTime;
        }
    }

    /**/
    /*
    MarioControllerP2::UpdateAnimation()

    NAME
        MarioControllerP2::UpdateAnimation() - Changes the animation Mario is in

    Synopsis
        void MarioControllerP2::UpdateAnimation(string attack);
            attack     --> what animation is going to be played

    Description
        Changes Mario's animation to fit how your moving or what attack is being used

    RETURNS
        Returns nothing
    */
    /**/
    private void UpdateAnimation(string attack)
    {
        //The enum that will correlate to what the user is trying to do
        characterState state;

        //The animation for the first jab attack
        if (attack == "jab1")
        {
            state = characterState.jab1;
        }

        //The animation for the second jab attack
        else if (attack == "jab2")
        {
            state = characterState.jab2;
        }

        //The animation for the third jab attack
        else if (attack == "jab3")
        {
            state = characterState.jab3;
        }

        //The animation for the dash attack
        else if (attack == "dashAtk")
        {
            state = characterState.dashAtk;
        }

        //The animation for the forward smash attack
        else if (attack == "FSmash")
        {
            state = characterState.FSmash;
        }

        //The animation for the neutral aerial attack
        else if (attack == "NAir")
        {
            state = characterState.NAir;
        }

        //The animation for the back aerial attack
        else if (attack == "BAir")
        {
            state = characterState.BAir;
        }

        //The animation for the down aerial attack
        else if (attack == "DAir")
        {
            state = characterState.DAir;
        }

        //The animation for the down smash attack
        else if (attack == "DSmash")
        {
            state = characterState.DSmash;
        }

        //The animation for the down tilt attack
        else if (attack == "DTilt")
        {
            state = characterState.DTilt;
        }

        //The animation for the forward aerial attack
        else if (attack == "FAir")
        {
            state = characterState.FAir;
        }

        //The animation for the down special cape attack
        else if (attack == "DownB")
        {
            state = characterState.DownB;
        }

        //The animation for the left and right tilt attack
        else if (attack == "SideTilt")
        {
            state = characterState.SideTilt;
        }

        //The animation for the up aerial attack
        else if (attack == "UpAir")
        {
            state = characterState.UpAir;
        }

        //The animation for the up b attack
        else if (attack == "UpB")
        {
            state = characterState.UpB;
        }

        //The animation for the up smash attack
        else if (attack == "UpSmash")
        {
            state = characterState.UpSmash;
        }

        //The animation for the up tilt attack
        else if (attack == "UpTilt")
        {
            state = characterState.UpTilt;
        }

        //The animation for when Mario is in special fall
        else if (attack == "SpFall")
        {
            state = characterState.SpFall;
        }

        //The animation for when Mario gets hurt
        else if (attack == "Hurt")
        {
            state = characterState.Hurt;
        }

        //If Mario is not attacking, in special fall, or getting hurt
        else
        {
            //If Mario is moving to the right
            if (dirX > 0f)
            {
                //Use the running animation
                state = characterState.running;

                //If Mario is on the ground make sure he is facing to the right
                if (IsOnGround())
                {
                    transform.localScale = new Vector3(scale, scale, scale);
                }
            }

            //If Mario is moving to the left
            else if (dirX < 0f)
            {
                //Use the running animation
                state = characterState.running;

                //If Mario is on the ground make sure he is facing to the left
                if (IsOnGround())
                {
                    transform.localScale = new Vector3(-1 * scale, scale, scale);
                }
            }

            //If not moving then play idle animation
            else
            {
                state = characterState.idle;
            }

            //If Mario is moving upwards play the jumping animation
            if (rb.velocity.y > .1f)
            {
                state = characterState.jumping;
            }

            //If Mario is moving downwards play the falling animation
            else if (rb.velocity.y < -.1f)
            {
                state = characterState.falling;
            }
        }

        //Gives the animation to play to the animator
        anim.SetInteger("state", (int)state);
    }

    /**/
    /*
    MarioControllerP2::IsOnGround()

    NAME
        MarioControllerP2::IsOnGround() - If Mario is on the ground

    Synopsis
        bool MarioControllerP2::IsOnGround();

    Description
        Checks if the bottom of Mario is touching the top of the stage

    RETURNS
        Returns true if Mario is touching the ground, false otherwise
    */
    /**/
    private bool IsOnGround()
    {
        return Physics2D.BoxCast(hurtBox.bounds.center, hurtBox.bounds.size, 0f, Vector2.down, .1f, stage);
    }

    /**/
    /*
    MarioControllerP2::KO()

    NAME
        MarioControllerP2::KO() - checks if Mario has been knocked of the stage

    Synopsis
        void MarioControllerP2::KO();

    Description
        Calls the death function if Mario has been knocked far enough off the screen in any direction

    RETURNS
        Returns nothing
    */
    /**/
    private void KO()
    {
        //If Mario goes too far left off screen
        if (rb.transform.position.x < -16)
        {
            Death();
        }

        //If Mario goes too far right off screen
        else if (rb.transform.position.x > 16)
        {
            Death();
        }

        //If Mario goes too far down off screen
        else if (rb.transform.position.y < -10)
        {
            Death();
        }

        //If Mario goes too far up off screen
        else if (rb.transform.position.y > 10)
        {
            Death();
        }
    }

    /**/
    /*
    MarioControllerP2::Death()

    NAME
        MarioControllerP2::Death() - Called once Mario is knocked far enough off stage to lose a stock

    Synopsis
        void MarioControllerP2::Death();

    Description
        Plays the sound for when a player gets knocked out. Removes one from the stock counter. The percent gets reset and Mario gets moved off screen for 3 seconds while waiting for him to respawn.
        Checks if Mario has 0 stocks and if so sets losingPlaying to player2 so the game can end.

    RETURNS
        Returns nothing
    */
    /**/
    private void Death()
    {
        //Plays the knockout sound
        PlaySound(knockout);
        stocks--;
        //Calls ResetPercent in the percent script to set Mario's percent back to 0
        playerPercent.ResetPercent();
        //Removes one stock image from the stocks at the bottom of the scene
        stockCount[stocks].enabled = false;
        //Moves Mario far off-screen while waiting for him to respawn
        rb.transform.position = new Vector3(100, 5, 0);
        //Stops Mario from moving anywhere while waiting for him to respawn
        rb.velocity = new Vector2(0, 0);
        //if Mario has run out of stocks make player2 the losing player

        if (stocks == 0)
        {
            PlayerPrefs.SetString("losingPlayer", "player2");
        }

        //Sets isDead to true and starts the death timer to delay Mario's respawn
        isDead = true;
        deathTimer = 6f;
    }

    /**/
    /*
    MarioControllerP2::CheckAttack()

    NAME
        MarioControllerP2::CheckAttack() - Checks for what input the user did

    Synopsis
        void MarioControllerP2::CheckAttack();

    Description
        Checks what attack the user is trying to use and if they are in the air or not.

    RETURNS
        Returns nothing
    */
    /**/
    private void CheckAttack()
    {
        //If Mario is on the ground
        if (IsOnGround())
        {
            //If the user presses right square bracket while holding down right arrow key or left arrow key, uses the forward smash attack
            if (Input.GetKeyDown(KeyCode.RightBracket) && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)))
            {
                //Sets dirX to 0 so Mario can't run while he attacks
                dirX = 0;
                inAnimation = true;
                //Sets the attack being used to FSmash
                attack = "FSmash";
                attackTimer = 1.33f;
                hitBoxStart = 0.8f;
                hitBoxEnd = 1f;
                return;
            }

            //If the user presses right bracket key while holding down the down arrow key, uses the down smash attack
            if (Input.GetKeyDown(KeyCode.RightBracket) && (Input.GetKey(KeyCode.DownArrow)))
            {
                //Sets dirX to 0 so Mario can't run while he attacks
                dirX = 0;
                inAnimation = true;
                //Sets the attack being used to DSmash
                attack = "DSmash";
                attackTimer = 1.45f;
                hitBoxStart = 0.5f;
                hitBoxEnd = 1.3f;
                return;
            }

            //If the user presses O while holding down the down arrow key, uses the down tilt attack
            if (Input.GetKeyDown(KeyCode.O) && (Input.GetKey(KeyCode.DownArrow)))
            {
                //Sets dirX to 0 so Mario can't run while he attacks
                dirX = 0;
                inAnimation = true;
                //Sets the attack being used to DTilt
                attack = "DTilt";
                attackTimer = 0.67f;
                hitBoxStart = 0.18f;
                hitBoxEnd = 0.5f;
                return;
            }

            //If the user presses the right square bracket key while holding down the up arrow key, uses the up smash attack
            if (Input.GetKeyDown(KeyCode.RightBracket) && (Input.GetKey(KeyCode.UpArrow)))
            {
                //Sets dirX to 0 so Mario can't run while he attacks
                dirX = 0;
                inAnimation = true;
                //Sets the attack being used to UpSmash
                attack = "UpSmash";
                attackTimer = 1.15f;
                hitBoxStart = 0.47f;
                hitBoxEnd = 0.7f;
                return;
            }

            //If the user presses O while holding down right arrow key or left arrow key, uses the side tilt attack
            if (Input.GetKeyDown(KeyCode.O) && ((Input.GetKey(KeyCode.RightArrow)) || (Input.GetKey(KeyCode.LeftArrow))))
            {
                //Sets dirX to 0 so Mario can't run while he attacks
                dirX = 0;
                inAnimation = true;
                //Sets the attack being used to SideTilt
                attack = "SideTilt";
                attackTimer = 0.6f;
                hitBoxStart = 0.15f;
                hitBoxEnd = 0.45f;
                return;
            }

            //If the user presses O while holding up arrow key, uses the up tilt attack
            if (Input.GetKeyDown(KeyCode.O) && (Input.GetKey(KeyCode.UpArrow)))
            {
                //Sets dirX to 0 so Mario can't run while he attacks
                dirX = 0;
                inAnimation = true;
                //Sets the attack being used to UpTilt
                attack = "UpTilt";
                attackTimer = 0.65f;
                hitBoxStart = 0.16f;
                hitBoxEnd = 0.4f;
                return;
            }
        }

        //If Mario is not on the ground
        if (!IsOnGround())
        {
            //If the user presses O, while facing right and holding left arrow key or while facing left and holding right arrow key
            if (Input.GetKeyDown(KeyCode.O) && ((transform.localScale.x > 0 && Input.GetKey(KeyCode.LeftArrow)) || (transform.localScale.x < 0 && Input.GetKey(KeyCode.RightArrow))))
            {
                inAnimation = true;
                inAerial = true;
                //Sets the attack being used to Back Aerial attack
                attack = "BAir";
                attackTimer = 0.7f;
                hitBoxStart = 0.1f;
                hitBoxEnd = 0.4f;
                return;
            }

            //If the user presses O, while facing left and holding left arrow key or while facing to the right and pressing right arrow key
            else if (Input.GetKeyDown(KeyCode.O) && ((transform.localScale.x < 0 && Input.GetKey(KeyCode.LeftArrow)) || (transform.localScale.x > 0 && Input.GetKey(KeyCode.RightArrow))))
            {
                inAnimation = true;
                inAerial = true;
                //Sets the attack being used to Forward Aerial attack
                attack = "FAir";
                attackTimer = 0.8f;
                hitBoxStart = 0.4f;
                hitBoxEnd = 0.7f;
                return;
            }

            //If the user presses O, while holding down arrow key
            else if (Input.GetKeyDown(KeyCode.O) && Input.GetKey(KeyCode.DownArrow))
            {
                inAnimation = true;
                inAerial = true;
                //Sets the attack being used to Down Aerial attack
                attack = "DAir";
                attackTimer = 0.8f;
                hitBoxStart = 0.6f;
                hitBoxEnd = 0.8f;
                return;
            }

            //If the user presses O, while holding up arrow key
            else if (Input.GetKeyDown(KeyCode.O) && Input.GetKey(KeyCode.UpArrow))
            {
                inAnimation = true;
                inAerial = true;
                //Sets the attack being used to Up Aerial attack
                attack = "UpAir";
                attackTimer = 0.5f;
                hitBoxStart = 0.1f;
                hitBoxEnd = 0.42f;
                return;
            }

            //If the user presses only O
            else if (Input.GetKeyDown(KeyCode.O))
            {
                inAnimation = true;
                inAerial = true;
                //Sets the attack being used to Neutral Aerial attack
                attack = "NAir";
                attackTimer = 0.7f;
                hitBoxStart = 0.15f;
                hitBoxEnd = 0.5f;
                return;
            }
        }

        //If the user presses P, while holding down arrow key
        if (Input.GetKeyDown(KeyCode.P) && (Input.GetKey(KeyCode.DownArrow)))
        {
            dirX = 0;
            inAnimation = true;
            //Sets the attack being used to Down B
            attack = "DownB";
            attackTimer = 0.8f;
            hitBoxStart = 0.27f;
            hitBoxEnd = 0.6f;
            return;
        }

        //If the user presses P, while holding down up arrow key
        if (Input.GetKeyDown(KeyCode.P) && (Input.GetKey(KeyCode.UpArrow)))
        {
            //Makes Mario face to the right if also holding down right arrow key, since normally he doesn't turn around while in the air
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.localScale = new Vector3(scale, scale, scale);
            }

            //Makes Mario face to the left if also holding down left arrow key, since normally he doesn't turn around while in the air
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.localScale = new Vector3(-1 * scale, scale, scale);
            }

            //If Mario is facing to the right then make him move to the right
            if (transform.localScale.x > 0)
            {
                dirX = 1f;
            }

            //If Mario is facing to the left then make him move to the left
            else if (transform.localScale.x < 0)
            {
                dirX = -1f;
            }
            //Makes Mario move a bit higher and farther to the right when using Up B
            rb.velocity = new Vector2(dirX * 3f, jumpHeight + 2);
            usedUpB = true;
            inAnimation = true;
            //Sets the attack being used to Up B
            attack = "UpB";
            hitBoxStart = 0.02f;
            hitBoxEnd = 0.5f;
            return;
        }

        //If Mario is not moving and is on the ground
        if (dirX == 0f && IsOnGround())
        {
            //If the user only pushes O
            if (Input.GetKeyDown(KeyCode.O))
            {
                //If the user presses O again before the jab2 timer ends it uses jab3
                if (jab2Timer >= 0)
                {
                    inAnimation = true;
                    //Sets the attack being used to jab3
                    attack = "jab3";
                    attackTimer = 0.8f;
                    hitBoxStart = 0.1f;
                    hitBoxEnd = 0.4f;
                    return;
                }

                //If the user presses O again before the jab1 timer ends it uses jab2
                else if (jab1Timer >= 0)
                {
                    inAnimation = true;
                    //Sets the attack being used to jab2
                    attack = "jab2";
                    attackTimer = 0.35f;
                    hitBoxStart = 0.05f;
                    hitBoxEnd = 0.25f;
                    jab2Timer = 0.5f;
                    return;
                }

                //The first time the user presses O uses jab1
                else
                {
                    inAnimation = true;
                    //Sets the attack being used to jab1
                    attack = "jab1";
                    attackTimer = 0.35f;
                    hitBoxStart = 0.05f;
                    hitBoxEnd = 0.25f;
                    jab1Timer = 0.5f;
                    return;
                }
            }
        }

        //If Mario is Moving left or right and is on the ground
        if (dirX != 0f && IsOnGround())
        {
            //If the user presses only P
            if (Input.GetKeyDown(KeyCode.P))
            {
                inAnimation = true;
                //Sets the attack being used to dash attack
                attack = "dashAtk";
                attackTimer = 0.92f;
                hitBoxStart = 0.1f;
                hitBoxEnd = 0.75f;
                return;
            }
        }

    }

    /**/
    /*
    MarioControllerP2::CheckAttackHit()

    NAME
        MarioControllerP2::CheckAttackHit() - Checks if the attack being used connects with the enemy

    Synopsis
        void MarioControllerP2::CheckAttackHit(string attack);
            attack      --> The attack being used by Mario

    Description
        Checks if the attack Mario used connected with the enemy. If so passes on the damage and knockback data to the enemy.

    RETURNS
        Returns nothing
    */
    /**/
    private void CheckAttackHit(string attack)
    {
        //The amount damage each attack does
        float damage = 0;
        //The distance each attack sends the opponent
        float distance = 0;
        //Where the opponent gets sent in the X direction
        float xDirection = 0;
        //Where the opponent gets sent in the Y direction
        float yDirection = 0;
        //How long the enemy gets stunned for when they're attacked
        float stunDuration = 0f;

        //Reduces the hitBoxEnd timer
        hitBoxEnd -= Time.deltaTime;

        //If Mario is in an animation but before or after the hitBox is active
        if (hitBoxStart > 0 || hitBoxEnd < 0)
        {
            hitBoxStart -= Time.deltaTime;
        }

        //If Mario is attacking and the hitbox is active
        else
        {
            //A box collider that will be set to the hitbox for the attack that was used
            BoxCollider2D attackUsed = null;
            //If the enemy has not already been hit by an attack
            if (!enemyHit)
            {
                //If the attack Mario is using is Jab1
                if (attack == "jab1")
                {
                    attackUsed = attackHitboxes[0];
                    damage = 2;
                    distance = 0;
                    yDirection = 0f;
                    stunDuration = 0.45f;
                }

                //If the attack Mario is using is Jab2
                else if (attack == "jab2")
                {
                    attackUsed = attackHitboxes[1];
                    damage = 2;
                    distance = 0;
                    yDirection = 0f;
                    stunDuration = 0.45f;
                }

                //If the attack Mario is using is Jab3
                else if (attack == "jab3")
                {
                    attackUsed = attackHitboxes[2];
                    damage = 6;
                    distance = 5;
                    yDirection = .5f;
                    stunDuration = 0.8f;
                    //If Mario is facing right knock the opponent to the right
                    if (transform.localScale.x > 0)
                    {
                        xDirection = .5f;
                    }
                    //If Mario is facing left knock the opponent to the left
                    else
                    {
                        xDirection = -0.5f;
                    }
                }

                //If the attack Mario is using is Back Air
                else if (attack == "BAir")
                {
                    attackUsed = attackHitboxes[3];
                    damage = 12;
                    distance = 6;
                    yDirection = .35f;
                    stunDuration = 0.8f;
                    //If Mario is facing right knock the opponent to the left
                    if (transform.localScale.x > 0)
                    {
                        xDirection = -0.65f;
                    }
                    //If Mario is facing left knock the opponent to the right
                    else
                    {
                        xDirection = .65f;
                    }
                }

                //If the attack Mario is using is Down Air
                else if (attack == "DAir")
                {
                    attackUsed = attackHitboxes[4];
                    damage = 8;
                    distance = 6;
                    yDirection = .8f;
                    stunDuration = 0.8f;
                    //If Mario is facing right knock the opponent to the right
                    if (transform.localScale.x > 0)
                    {
                        xDirection = .2f;
                    }
                    //If Mario is facing left knock the opponent to the left
                    else
                    {
                        xDirection = -0.2f;
                    }
                }

                //If the attack Mario is using is Dash attack
                else if (attack == "dashAtk")
                {
                    attackUsed = attackHitboxes[5];
                    damage = 9;
                    distance = 7;
                    yDirection = .65f;
                    stunDuration = 0.8f;
                    //If Mario is facing right knock the opponent to the right
                    if (transform.localScale.x > 0)
                    {
                        xDirection = .35f;
                    }
                    //If Mario is facing left knock the opponent to the left
                    else
                    {
                        xDirection = -0.35f;
                    }
                }

                //If the attack Mario is using is Down Smash
                else if (attack == "DSmash")
                {
                    attackUsed = attackHitboxes[6];
                    damage = 12;
                    distance = 7;
                    yDirection = .3f;
                    stunDuration = 0.8f;
                    //If Mario is facing right knock the opponent to the right
                    if (transform.localScale.x > 0)
                    {
                        xDirection = .7f;
                    }
                    //If Mario is facing left knock the opponent to the left
                    else
                    {
                        xDirection = -0.7f;
                    }
                }

                //If the attack Mario is using is Down Tilt
                else if (attack == "DTilt")
                {
                    attackUsed = attackHitboxes[7];
                    damage = 8;
                    distance = 5;
                    yDirection = .8f;
                    stunDuration = 0.8f;
                    //If Mario is facing right knock the opponent to the right
                    if (transform.localScale.x > 0)
                    {
                        xDirection = .2f;
                    }
                    //If Mario is facing left knock the opponent to the left
                    else
                    {
                        xDirection = -0.2f;
                    }
                }

                //If the attack Mario is using is Forward Air
                else if (attack == "FAir")
                {
                    attackUsed = attackHitboxes[8];
                    damage = 15;
                    distance = 6;
                    yDirection = .5f;
                    stunDuration = 0.8f;
                    //If Mario is facing right knock the opponent to the right
                    if (transform.localScale.x > 0)
                    {
                        xDirection = .5f;
                    }
                    //If Mario is facing left knock the opponent to the left
                    else
                    {
                        xDirection = -0.5f;
                    }
                }

                //If the attack Mario is using is Forward Smash
                else if (attack == "FSmash")
                {
                    attackUsed = attackHitboxes[9];
                    damage = 21;
                    distance = 8;
                    yDirection = .5f;
                    stunDuration = 0.8f;
                    //If Mario is facing right knock the opponent to the right
                    if (transform.localScale.x > 0)
                    {
                        xDirection = .5f;
                    }
                    //If Mario is facing left knock the opponent to the left
                    else
                    {
                        xDirection = -0.5f;
                    }
                }

                //If the attack Mario is using is Neutral Air
                else if (attack == "NAir")
                {
                    attackUsed = attackHitboxes[10];
                    damage = 7;
                    distance = 3;
                    yDirection = .4f;
                    stunDuration = 0.8f;
                    //If Mario is facing right knock the opponent to the right
                    if (transform.localScale.x > 0)
                    {
                        xDirection = .6f;
                    }
                    //If Mario is facing left knock the opponent to the left
                    else
                    {
                        xDirection = -0.6f;
                    }
                }

                //If the attack Mario is using is Down B
                else if (attack == "DownB")
                {
                    attackUsed = attackHitboxes[11];
                    damage = 8;
                    distance = 0;
                    yDirection = .4f;
                    stunDuration = 0.8f;
                    //If Mario is facing right knock the opponent to the right
                    if (transform.localScale.x > 0)
                    {
                        xDirection = .6f;
                    }
                    //If Mario is facing left knock the opponent to the left
                    else
                    {
                        xDirection = -0.6f;
                    }
                }

                //If the attack Mario is using is Side Tilt
                else if (attack == "SideTilt")
                {
                    attackUsed = attackHitboxes[12];
                    damage = 8;
                    distance = 6;
                    yDirection = .4f;
                    stunDuration = .7f;
                    //If Mario is facing right knock the opponent to the right
                    if (transform.localScale.x > 0)
                    {
                        xDirection = .6f;
                    }
                    //If Mario is facing left knock the opponent to the left
                    else
                    {
                        xDirection = -0.6f;
                    }
                }

                //If the attack Mario is using is Up Air
                else if (attack == "UpAir")
                {
                    attackUsed = attackHitboxes[13];
                    damage = 7;
                    distance = 4;
                    yDirection = .8f;
                    stunDuration = 0.8f;
                    //If Mario is facing right knock the opponent to the right
                    if (transform.localScale.x > 0)
                    {
                        xDirection = .2f;
                    }
                    //If Mario is facing left knock the opponent to the left
                    else
                    {
                        xDirection = -0.2f;
                    }
                }

                //If the attack Mario is using is Up B
                else if (attack == "UpB")
                {
                    attackUsed = attackHitboxes[14];
                    damage = 8;
                    distance = 6;
                    yDirection = .7f;
                    stunDuration = 0.8f;
                    //If Mario is facing right knock the opponent to the right
                    if (transform.localScale.x > 0)
                    {
                        xDirection = .3f;
                    }
                    //If Mario is facing left knock the opponent to the left
                    else
                    {
                        xDirection = -0.3f;
                    }
                }

                //If the attack Mario is using is Up Smash
                else if (attack == "UpSmash")
                {
                    attackUsed = attackHitboxes[15];
                    damage = 18;
                    distance = 5;
                    yDirection = .9f;
                    stunDuration = 0.8f;
                    //If Mario is facing right knock the opponent to the right
                    if (transform.localScale.x > 0)
                    {
                        xDirection = .1f;
                    }
                    //If Mario is facing left knock the opponent to the left
                    else
                    {
                        xDirection = -0.1f;
                    }
                }

                //If the attack Mario is using is Up Tilt
                else if (attack == "UpTilt")
                {
                    attackUsed = attackHitboxes[16];
                    damage = 6;
                    distance = 4;
                    xDirection = 0f;
                    yDirection = 1f;
                    stunDuration = 0.8f;
                }

                //Checks if the attack hitbox overlaps the opponents hurtbox
                Collider2D hit = Physics2D.OverlapBox(attackUsed.bounds.center, attackUsed.bounds.extents, 0f, LayerMask.GetMask("Char1"));

                //If the attack hits the enemy
                if (hit != null)
                {
                    //Tells the opponents percent script to take damage
                    hit.SendMessageUpwards("TakeDamage", damage);
                    //Sets enemy hit to true so one attack cannot hit multiple times
                    enemyHit = true;
                    //Plays the hit sound effect
                    PlaySound(hitSound);

                    //Tells the opponents script that they were hit and how far back to knock them back
                    hit.SendMessageUpwards("Knockback", new Vector4(distance, xDirection, yDirection, stunDuration));
                }
            }
        }
    }

    /**/
    /*
    MarioControllerP2::Knockback()

    NAME
        MarioControllerP2::Knockback() - Is called when the enemy hits Mario

    Synopsis
        void MarioControllerP2::Knockback(Vector4 knockInfo);
            knockInfo      --> A vector4 that contains the information for knowing how far and at what angle Mario should get knocked back to 

    Description
        Gets the percent that Mario is at. Plays the hurt animation and knocks Mario back depending on knockInfo.

    RETURNS
        Returns nothing
    */
    /**/
    private void Knockback(Vector4 knockInfo)
    {
        //knockInfo.x is the distance the attack will send Mario
        float distance = knockInfo.x;
        //knockInfo.y is the X direction that Mario will get sent in
        float xDirection = knockInfo.y;
        //knockInfo.z is the Y direction that Mario will get sent in
        float yDirection = knockInfo.z;
        //knockInfo.w is the length of time Mario is stunned for
        float stunDuration = knockInfo.w;
        //Gets the percent that Mario is currently at
        float percent = playerPercent.GetPercent();

        //Sets Mario's stun timer to how long the attack stuns Mario for
        attackStunTimer = stunDuration;
        //Plays the hurt animation
        UpdateAnimation("Hurt");

        //Makes Mario get knocked back in the direction and distance it has to
        rb.velocity = new Vector2((percent / 30) * distance * xDirection + (5 * xDirection), (percent / 30) * distance * yDirection + (5 * yDirection));
    }

    /**/
    /*
    MarioControllerP2::PlaySound()

    NAME
        MarioControllerP2::PlaySound() - Plays the sound clip that is passed to it

    Synopsis
        void MarioControllerP2::PlaySound(AudioClip clip);
            clip     --> The audio clip to be played

    Description
        Plays the sound clip that is passed to it

    RETURNS
        Returns nothing
    */
    public void PlaySound(AudioClip clip)
    {
        soundEffects.PlayOneShot(clip);
    }
}