﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    private enum State {Attack, Patrol};
    public GameObject[] enemyList; 
    private GameObject child;

    public float movementSmooth = 0.05f; 

    private Transform player; 
    public float viewRadius; 
    public float attackRad;
    

    private Animator animator;

    private Rigidbody2D rb;
    public float enemyVelocity; 
    public Vector3 refVel; 
    

    private State state;
    // Start is called before the first frame update
    void Start()
    {

        GameObject character = GameObject.FindGameObjectsWithTag("Player")[0];
        player = character.transform; 
        //Remove Temp Child
        foreach (Transform child in this.transform){
            GameObject.Destroy(child.gameObject);
        }

        //Spawn Enemy
        int enemyIndex = Random.Range(0,4);
        child = Instantiate(enemyList[enemyIndex],transform.position, transform.rotation,transform);
        animator = child.GetComponent<Animator>(); 
        //Get Component
        rb = GetComponent<Rigidbody2D>();
        state = State.Patrol;                 
    }


    void checkState(){
        float x = Mathf.Abs(player.position.x - this.transform.position.x);
        float y = Mathf.Abs(player.position.y - this.transform.position.y);
        if (x * x + y *y <= viewRadius * viewRadius  ){
            state = State.Attack; 
        }
        else{
            state = State.Patrol;
        }   
    }

    void flip(){
        Vector3 scale = transform.localScale; 
        scale.x *= -1; 
        transform.localScale = scale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {            
        checkState();
        Vector3 tgtVel = new Vector2(0,0);
        // If player is seen 
        if (state == State.Attack){
            float playerDist = player.position.x - this.transform.position.x;
            //Get closer to player

            if (Mathf.Abs(playerDist) >= attackRad ){
                tgtVel = new Vector2 (enemyVelocity * (playerDist/Mathf.Abs(playerDist)) , 0); 
                rb.velocity = Vector3.SmoothDamp(rb.velocity, tgtVel, ref refVel,movementSmooth);
                animator.SetFloat("speed", enemyVelocity);
                if (rb.velocity.x < 0 && transform.localScale.x > 0){
                    flip();
                }
                if (rb.velocity.x > 0 && transform.localScale.x < 0){
                    flip();
                }
            }
            //If close, stop following and attack 
            else{
                rb.velocity = new Vector2(0,0); 
                animator.SetFloat("speed", 0); 
            }
        }
        if (state == State.Patrol){
            rb.velocity = new Vector2(0,0); 
            animator.SetFloat("speed", 0); 
        }
       
    }
}
 