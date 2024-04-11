using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UBlocklyGame.Maze
{
    public class MazeAvatar : MonoBehaviour
    {
        private Transform mTrans;
        public bool left;
        public bool right;
        public bool front;

        public Transform rightLineSensor;
        public Transform leftLineSensor;
        public bool rightLine;
        public bool leftLine;
        public bool lineBelow;


        private void Awake()
        {
            mTrans = transform;
        }

        public void Update()
        {
            front = WallFront();
            left = WallLeft();
            right = WallRight();
            leftLine = LeftLine();
            rightLine = RightLine();
            lineBelow = IsLineBelow();
        }

        private bool RightLine()
        {
            return Physics.Raycast(rightLineSensor.position, transform.TransformDirection(Vector3.down), out var hit, 2f) && hit.transform.gameObject.tag == "WayLine";
        }

        private bool LeftLine()
        {
            return Physics.Raycast(leftLineSensor.position, transform.TransformDirection(Vector3.down), out var hit, 2f) && hit.transform.gameObject.tag == "WayLine";
        }

        public bool WallFront()
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 2f, Color.green);

            return Physics.Raycast(mTrans.position + Vector3.left, transform.TransformDirection(Vector3.forward), out var hit, 2f) ||
                Physics.Raycast(mTrans.position, transform.TransformDirection(Vector3.forward), out var hit1, 2f) ||
                Physics.Raycast(mTrans.position + Vector3.right, transform.TransformDirection(Vector3.forward), out var hit2, 2f);
        }

        public bool WallLeft()
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left * 2f), Color.green);
            return Physics.Raycast(mTrans.position + Vector3.forward, transform.TransformDirection(Vector3.left), out var hit, 2f) ||
                Physics.Raycast(mTrans.position, transform.TransformDirection(Vector3.left), out var hit1, 2f) ||
                Physics.Raycast(mTrans.position + Vector3.back, transform.TransformDirection(Vector3.left), out var hit2, 2f);
        }

        public bool WallRight()
        {
            Debug.DrawRay(transform.position + Vector3.forward, transform.TransformDirection(Vector3.right * 2f), Color.green);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right * 2f), Color.green);
            Debug.DrawRay(transform.position + Vector3.back, transform.TransformDirection(Vector3.right * 2f), Color.green);

            return Physics.Raycast(mTrans.position + Vector3.forward, transform.TransformDirection(Vector3.right), out var hit, 2f) ||
            Physics.Raycast(mTrans.position, transform.TransformDirection(Vector3.right), out var hit1, 2f) ||
            Physics.Raycast(mTrans.position + Vector3.back, transform.TransformDirection(Vector3.right), out var hit2, 2f);
        }

        public IEnumerator MoveForward(float distance)
        {
            if (WallFront()) yield return null;

            yield return Move(mTrans.position + mTrans.TransformDirection(Vector3.forward * Mathf.Abs(distance)));
        }

        public IEnumerator MoveForwardByLine()
        {
            //TODO Проверка на наличие линии в принципе

            if(IsLineBelow())
            {
                if (!leftLine && !rightLine)
                {
                    yield return Move(mTrans.position + mTrans.TransformDirection(Vector3.forward / 5), 1/5f);
                }
                else if (leftLine)
                {
                    yield return Turn(-5);
                }
                else if (rightLine)
                {
                    yield return Turn(5);
                }
            }
        }

        public bool IsLineBelow()
        {
            Vector3 left = leftLineSensor.position;
            Vector3 right = rightLineSensor.position;

            List<Vector3> rt = new List<Vector3>();

            for(int i = 0; i < 20; i++)
            {
                rt.Add(Vector3.Lerp(left, right, i / 19f));
            }

            bool hasLine = false;

            List<RaycastHit> hits = new List<RaycastHit>();
            foreach(var vector in rt)
            {
                Debug.DrawRay(vector, transform.TransformDirection(Vector3.down), Color.red);

                if (Physics.Raycast(vector, transform.TransformDirection(Vector3.down), out var hit, 3f) && hit.transform.gameObject.tag == "WayLine")
                {
                    hits.Add(hit);
                    hasLine = true;
                    break;
                }
            }

            return hasLine;
        }

        public void Init(Vector3 startPos, Vector3 startDir)
        {
            mTrans.position = startPos;
            mTrans.forward = startDir;
        }

        public IEnumerator Move(Vector3 targetPos, float speedmultiplier = 1)
        {
            float distance = Vector3.Distance(targetPos, mTrans.position);
            Vector3 dir = (targetPos - mTrans.position).normalized;
            float speed = distance / (MazeDefine.MOVE_SPEED * speedmultiplier / MazeDefine.SPEED_FACTOR);
            while (distance > 0)
            {
                float move = Mathf.Min(distance, speed * Time.deltaTime);
                mTrans.Translate(move * dir, Space.World);
                distance -= move;

                yield return null;
            }
        }

        public IEnumerator Turn(float angle)
        {
            float speed = angle / (MazeDefine.TURN_SPEED / MazeDefine.SPEED_FACTOR);
            while (Mathf.Abs(angle) > 0)
            {
                float turn = speed * Time.deltaTime;
                if (Mathf.Abs(turn) > Mathf.Abs(angle))
                    turn = angle;
                
                mTrans.Rotate(Vector3.up, turn, Space.Self);
                angle -= turn;

                yield return null;
            }
        }
    }
}