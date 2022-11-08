using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");//获取水平轴输入
        float y = Input.GetAxis("Vertical");//获取垂直轴输入
        transform.Translate(Vector3.right * x * moveSpeed * Time.deltaTime, Space.World);
        transform.Translate(Vector3.up * y * moveSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("EndPoint"))
        {
            Debug.Log("Touched End Point, Teleporting to next room");
            SceneManager.LoadScene(0);
        }
    }
}