using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle
{
    public List<Vector2> gestureDetector = new List<Vector2>();
    public int gestureCount = 0;
    Vector2 gestureSum = Vector2.zero;
    public int gestureLength = 0;
    public int numOfCircleToShow = 1;
    private bool hasDrawn = false;  // 新增标志位

    public bool isGestureDone()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touches.Length != 1)
            {
                gestureDetector.Clear();
                gestureCount = 0;
                hasDrawn = false;  // 重置标志位
            }
            else
            {
                if (Input.touches[0].phase == TouchPhase.Canceled || Input.touches[0].phase == TouchPhase.Ended)
                {
                    gestureDetector.Clear();
                    hasDrawn = false;  // 松开时重置标志位
                }
                else if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    Vector2 p = Input.touches[0].position;
                    if (gestureDetector.Count == 0 || (p - gestureDetector[gestureDetector.Count - 1]).magnitude > 10)
                        gestureDetector.Add(p);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))  // 鼠标松开时重置
            {
                gestureDetector.Clear();
                hasDrawn = false;
            }
            else
            {
                if (Input.GetMouseButton(0))  // 鼠标按住时
                {
                    Vector2 p = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    if (gestureDetector.Count == 0 || (p - gestureDetector[gestureDetector.Count - 1]).magnitude > 10)
                        gestureDetector.Add(p);
                }
            }
        }

        if (gestureDetector.Count < 10)  // 手势点不足，返回
            return false;

        // 如果已经完成了手势，就不再处理
        if (hasDrawn)
            return false;

        gestureSum = Vector2.zero;
        gestureLength = 0;
        Vector2 prevDelta = Vector2.zero;
        for (int i = 0; i < gestureDetector.Count - 2; i++)
        {
            Vector2 delta = gestureDetector[i + 1] - gestureDetector[i];
            float deltaLength = delta.magnitude;
            gestureSum += delta;
            gestureLength += (int)deltaLength;

            float dot = Vector2.Dot(delta, prevDelta);
            if (dot < 0f)
            {
                gestureDetector.Clear();
                gestureCount = 0;
                return false;
            }

            prevDelta = delta;
        }

        int gestureBase = (Screen.width + Screen.height) / 4;

        if (gestureLength > gestureBase && gestureSum.magnitude < gestureBase / 2)
        {
            gestureDetector.Clear();
            gestureCount++;
            hasDrawn = true;  // 完成手势后标记为已画完
            if (gestureCount >= numOfCircleToShow)
                return true;
        }

        return false;
    }

}
