using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class PlayerScrp : MonoBehaviour
{
    #region Variáveis
    public float forcaPulo;
    public float velocidadeMaxima;
    public bool isGrounded;
    public float mult;
    public int puloduplo;
    public int rast;
    public string LeftAlt;
    public float contador;
    public Transform bulletSpawn;
    public float fireRate;
    public float nextFire;
    public GameObject bulletObject;
    public Text timerText;
    private float currentTime = 0;
    private float step = 0.1f;
    private bool time;
    public int vida = 100;
    public float damage;
    private int dale;
    private bool isDead = false;
    private SpriteRenderer sprite;
    private Vector3 startPosition;
    public float invecibilityTime;
    public GameObject gameOverText;
    public GameObject restartButton;
    Renderer rend;
    public Heart heart;
    public Heart2 heart2;
    public Heart3 heart3;
    Color c;


    
    
    
    #endregion

    void Start()
    {
       
        gameOverText.SetActive(false);
        restartButton.SetActive(false);
        rend = GetComponent<Renderer>();
        c = rend.material.color;
       

    }   
   

    #region IEnumerator Morte ()
    IEnumerator Morte ()
    {
        yield return new WaitForSeconds(2);
        GetComponent<Animator>().SetBool("DeadAdv", false);
        GetComponent<SpriteRenderer>().enabled = false;
        gameOverText.SetActive(true);
        restartButton.SetActive(true);
    }
    #endregion

    void Update()
    {
                                
        #region Movimento e Flip      
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        float movimento = Input.GetAxis("Horizontal");
        rigidbody.velocity = new Vector2(movimento * velocidadeMaxima, rigidbody.velocity.y);

        if (movimento < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;

        }
        else if (movimento > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;


        }

        if (movimento > 0 || movimento < 0)
        {
            GetComponent<Animator>().SetBool("walking", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("walking", false);
        }
        #endregion
        #region Função Pulo
      
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, forcaPulo));
            GetComponent<Animator>().SetBool("jumping", true);

        }
        #endregion
        #region Arco e Flecha

        if (Input.GetKeyDown(KeyCode.E))
        {

            GetComponent<Animator>().SetTrigger("bow");
        }
        #endregion
        #region Espada       
        contador = Random.value;
        if (Input.GetKeyDown(KeyCode.Q) && contador < 0.3)
        {
            GetComponent<Animator>().SetTrigger("sword");

        }
        else if (Input.GetKeyDown(KeyCode.Q) && contador > 0.6)
        {
            GetComponent<Animator>().SetTrigger("sword2");
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            GetComponent<Animator>().SetTrigger("sword3");
        }
        #endregion
        #region Double Jump       
        if (isGrounded == true)
        {
            GetComponent<Animator>().SetBool("doublejump", false);
            puloduplo = 1;
        }

        if (Input.GetKeyDown(KeyCode.Space) && (puloduplo > 0))
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, mult * 100));
            GetComponent<Animator>().SetBool("doublejump", true);
            puloduplo--;


        }
        #endregion
        #region Rasteira        


        if (Input.GetKeyDown(KeyCode.LeftControl) && (isGrounded == true))

        {

            GetComponent<Rigidbody2D>().AddForce(new Vector2(rast, 0));
            GetComponent<Animator>().SetBool("roll", true);

        }


        if (GetComponent<SpriteRenderer>().flipX == true && (Input.GetKeyDown(KeyCode.LeftControl)) && isGrounded)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(-rast * 2, 0));

        }
        #endregion
        #region Correndo       

        if (Input.GetButton("leftShift") && (movimento > 0 || movimento < 0))
        {
            rigidbody.velocity = new Vector2(movimento * (velocidadeMaxima * 2), rigidbody.velocity.y);
            GetComponent<Animator>().SetBool("running", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("running", false);
        }
        #endregion
        #region Matar Inimigo / Espada Animação

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GetComponent<Animator>().SetTrigger("sword");
            Collider2D[] colliders = new Collider2D[3];
            transform.Find("SwordArea").gameObject.GetComponent<Collider2D>()
                .OverlapCollider(new ContactFilter2D(), colliders);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null && colliders[i].gameObject.CompareTag("Monstros"))
                {

                    Destroy(colliders[i].gameObject);
                }
            }
        }
        #endregion
        #region Tiro
        // Tiroh


        time = false;
        if (Input.GetKeyDown(KeyCode.E) && Time.time > nextFire)
        {
            
            Fire();     
        }

        #endregion
        #region morte Player
        if (vida <= 0)
        {
            dale = 5;
            GetComponent<Animator>().SetBool("DeadAdv", true);
            GetComponent<Animator>().SetBool("morto", true);
           StartCoroutine(Morte());
           
        }
        else
        {
            GetComponent<Animator>().SetBool("DeadAdv", false);
        }
        #endregion


    }
   
    

    #region Entrar e Sair de Colisão
    // Entrar e Sair de Colisão

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("AxeArea") && dale != 5)
        {
            vida --;
            if (vida == 2)
            {
                heart.desativar();
            }
            if (vida == 1)
            {
                heart2.desativar();
            }
            if (vida == 0)
            {
                heart3.desativar();
            }
            GetComponent<Animator>().SetBool("damage", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("damage", false);
        }

        if (other.gameObject.name.Equals ("Axe area") && vida > 0)
        {
            StartCoroutine("Invulneravel");
        }
    }
    void OnCollisionEnter2D(Collision2D collision2D)
    {

        if (collision2D.gameObject.CompareTag("Plataformas"))
        {
            isGrounded = true;
            GetComponent<Animator>().SetBool("jumping", false);
        }

        if (collision2D.gameObject.CompareTag("Beast"))
        {
            vida = 0;
            heart.desativar();
            heart2.desativar();
            heart3.desativar();

        }
    }
    void OnCollisionExit2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Plataformas"))
        {
            isGrounded = false;

        }

    }
    #endregion

    #region void Flip e void Fire
    void Flip()
    {
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        if (!GetComponent<SpriteRenderer>().flipX)
        {
            bulletSpawn.position = new Vector3(this.transform.position.x + 0.1f, bulletSpawn.transform.position.y, bulletSpawn.transform.position.z);
        }
        else
        {
            bulletSpawn.position = new Vector3(this.transform.position.x - 0.1f, bulletSpawn.transform.position.y, bulletSpawn.transform.position.z);
        }

    }

    void Fire()
    {
        GetComponent<Animator>().SetTrigger("bow");
        nextFire = Time.time + fireRate;
        GameObject cloneBullet = Instantiate(bulletObject, bulletSpawn.position, bulletSpawn.rotation);

        if (GetComponent<SpriteRenderer>().flipX)
        {
            cloneBullet.transform.eulerAngles = new Vector3(0, 0, 180);
        }




    }
    #endregion

    #region Invulneravel
    IEnumerator Invulneravel ()
    {
        Physics2D.IgnoreLayerCollision(8, 10, true);
        c.a = 0.5f;
        rend.material.color = c;       
        yield return new WaitForSeconds(3f);
        Physics2D.IgnoreLayerCollision(8, 10, false);
        c.a = 1f;
        rend.material.color = c;

    }
    #endregion 





}
