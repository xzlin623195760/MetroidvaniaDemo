using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    public bool jumping = false;
    public bool dashing = false;
    public bool recoilingX = false;
    public bool recoilingY = false;
    public bool lookingRight = true;
    public bool invincible;
    public bool healing;
    public bool casting;
    public bool cutScene = false; // �л�����
    public bool alive = true; // ��ɫ���״̬
}