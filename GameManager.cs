using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject enemy;
    [SerializeField]
    GameObject bulletPrefab;
    
    // === Player Atribute ===
    [SerializeField]
    float pl_fireInterval;
    float pl_fireCountDown = 0f;
    [SerializeField]
    float pl_moveSpeed;

    // === Bullet Atribute ====
    [SerializeField]
    int max_bullets;
    GameObject[] bullets;
    Vector3[] bl_velocities;
    float[] bl_age;
    float[] bl_period;
    bool[] bl_hit;
    [SerializeField]
    float bl_initialVelocities;
    [SerializeField]
    float bl_acceleration;
    [SerializeField]
    float bl_initialPeriod;
    [SerializeField]
    float bl_lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        // make arraies
        bullets = new GameObject[max_bullets];
        bl_velocities = new Vector3[max_bullets];
        bl_age = new float[max_bullets];
        bl_period = new float[max_bullets];
        bl_hit = new bool[max_bullets];

        // make bullets
        for(int i=0; i<bullets.Length; i++){
            bullets[i] = Instantiate(bulletPrefab);
            bullets[i].SetActive(false);
            bl_velocities[i] = Vector3.zero;
            bl_period[i] = bl_initialPeriod;
            bl_age[i] = 0f;
            bl_hit[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

        #region Player
        #region Player Move
        float hori = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        Vector3 pl_nowPos = player.transform.position;
        Vector3 pl_newPos = player.transform.position;
        pl_newPos.x += pl_moveSpeed * hori * Time.deltaTime;
        pl_newPos.y += pl_moveSpeed * vert * Time.deltaTime;
        if(Mathf.Abs(pl_newPos.x) > 6f) pl_newPos.x = pl_nowPos.x;
        if(Mathf.Abs(pl_newPos.y) > 4.5f) pl_newPos.y = pl_nowPos.y;
        player.transform.position = pl_newPos;
        #endregion
        #region Player Shot
        if(pl_fireCountDown > pl_fireInterval){
            pl_fireCountDown-= pl_fireInterval;
            // Front
            for(int i=0; i<bullets.Length; i++){
                // find destroyed bullet
                if(!bullets[i].activeInHierarchy){
                    bullets[i].SetActive(true);
                    Vector3 insPos = player.transform.position;
                    insPos.x += 1;
                    bullets[i].transform.position = insPos;
                    Vector3 d1 = new Vector3(1,0,0);
                    bl_velocities[i] = d1 * bl_initialVelocities;
                    break;
                }
            }
            // Up
            for(int i=0; i<bullets.Length; i++){
                // find destroyed bullet
                if(!bullets[i].activeInHierarchy){
                    bullets[i].SetActive(true);
                    Vector3 insPos = player.transform.position;
                    insPos.x += 1;
                    bullets[i].transform.position = insPos;
                    Vector3 d1 = new Vector3(1,1,0).normalized;
                    bl_velocities[i] = d1 * bl_initialVelocities;
                    break;
                }
            }

            // Down
            for(int i=0; i<bullets.Length; i++){
                // find destroyed bullet
                if(!bullets[i].activeInHierarchy){
                    bullets[i].SetActive(true);
                    Vector3 insPos = player.transform.position;
                    insPos.x += 1;
                    bullets[i].transform.position = insPos;
                    Vector3 d1 = new Vector3(1,-1,0).normalized;
                    bl_velocities[i] = d1 * bl_initialVelocities;
                    break;
                }
            }
        }
        #endregion
        #endregion


        #region Bullet
        #region Bullet Move
        for(int i=0; i < bullets.Length; i++){
            if(bullets[i].activeInHierarchy){
                Vector3 bl_nowPos = bullets[i].transform.position;
                Vector3 bl_newPos = bullets[i].transform.position;
                Vector3 bl_direction = (enemy.transform.position - bullets[i].transform.position);
                if(bl_direction.magnitude < 1f) bl_hit[i] = true; // hit enemy
                if(bl_nowPos.magnitude > 10f) bl_hit[i] = true; // go to space

                Vector3 newAcc = Vector3.zero;
                // // *** 固定加速度
                // bl_acceleration = 400f;
                // newAcc += bl_direction.normalized * bl_acceleration;

                // *** 物理に基づく
                // newAcc += (bl_direction - bl_velocities[i] * bl_period[i])* 2f / (bl_period[i]*bl_period[i]);
                // if(newAcc.magnitude > 100f){
                //     newAcc = newAcc.normalized * 100f;
                // }

                // *** 物理に基づく　敵との前後関係で加速度が変わる
                float maxAcc = 0f;
                float dot = Vector2.Dot( // 内積を取る。dotの値は-1～1
                    bl_nowPos.normalized,
                     enemy.transform.position.normalized);
                dot = dot * -1; // 符号反転
                float pow = (dot+1)/2; // 内積の値を0～1に変更
                maxAcc = 0.1f; // 目の前にいる時の加速度
                maxAcc += 16000f * pow; // 真後ろにいる時の加速度
                newAcc += (bl_direction - bl_velocities[i] * bl_period[i])* 2f / (bl_period[i]*bl_period[i]);
                if(newAcc.magnitude > maxAcc){
                    newAcc = newAcc.normalized * maxAcc;
                }



                bl_velocities[i] += newAcc * Time.deltaTime;
                bl_newPos += bl_velocities[i] * Time.deltaTime;
                bullets[i].transform.position = bl_newPos;
            }
        }
        #endregion
        #endregion


        #region Enemy
        #region Enemy Move
        Vector2 en_rad = new Vector2(3f, 2f);
        Vector2 en_spd = new Vector2(0.1f,0.5f);
        Vector3 en_nowPos = enemy.transform.position;
        Vector3 en_newPos = enemy.transform.position;
        en_newPos.x = en_rad.x * Mathf.Cos(en_spd.x * Time.time);
        en_newPos.y = en_rad.x * Mathf.Sin(en_spd.y * Time.time);
        enemy.transform.position = en_newPos;
        #endregion
        #endregion
    }
    void FixedUpdate()
    {

    }

    void LateUpdate(){
        pl_fireCountDown += Time.deltaTime;
        for(int i=0; i < bullets.Length; i++){
            if(bullets[i].activeInHierarchy){
                bl_age[i] += Time.deltaTime;
                bl_period[i]-=Time.deltaTime;
                if(bl_age[i] > bl_lifeTime) destroyBullet(i);
                if(bl_hit[i]) destroyBullet(i);
            }
        }
    }

    void destroyBullet(int i){
        bullets[i].SetActive(false);
        bl_age[i] = 0f;
        bl_period[i] = bl_initialPeriod;
        bl_hit[i] = false;
    }
}
