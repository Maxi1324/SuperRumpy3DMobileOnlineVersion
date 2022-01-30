using Camera2.Bad;
using Obstacles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Player.Abilities
{
    public class GrapplingHook : MovementAbility
    {
        public PlayerManager PManager;
        public float hookRadius;

        public float AnimTime = 1;
        public float AnimHeight = 2;
        public int AnimQuality = 5;

        public Transform Hips;

        private Transform hookPoint;
        private float AnimTimer = 0;

        private float lastDis;

        private void Reset()
        {
            PManager = GetComponent<PlayerManager>();
        }

        public override bool Active(float distance, InteractPlayer ob, bool allowed)
        {
            if (hookRadius > distance)
            {
                if (allowed)
                {
                    if (ob.key == "GrapplingPoint" && hookPoint == null && PManager.Fire3)
                    {
                        hookPoint = ob.transform;
                        Debug.Log("hook");
                        PManager.AllowedMoves = 2;
                        lastDis = distance;
                        return true;
                    }
                }
            }
            return false;
        }

        private void Start()
        {
            NeedObject = true;

            SpringJoint joint = GetComponent<SpringJoint>();

            gameObject.AddComponent<SpringJoint>();
            joint = GetComponent<SpringJoint>();

            joint.anchor = Vector3.zero;
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = Vector3.zero;
            joint.spring = 1.25f;
        }

        private void LateUpdate()
        {
            if (hookPoint != null)
            {
                PManager.AllowedMoves = 2;


                LineRenderer renderer = PManager.PInfo.LineRenderer;
                AnimTimer += Time.deltaTime;

                if (!(AnimTimer > AnimTime))
                {

                    Vector3 Line = hookPoint.transform.position - transform.position;
                    renderer.positionCount = (int)(Line.magnitude) * AnimQuality;
                    Vector3 NLine = Line.normalized;
                    float length = Line.magnitude / AnimTime;
                    Vector3 lastPos = Vector3.zero;

                    Quaternion quat = Quaternion.LookRotation(NLine, Vector3.up);
                    for (int i = 0; i < renderer.positionCount; i++)
                    {
                        Vector3 PPosOr = (NLine / AnimQuality) * i;
                        Debug.DrawRay(transform.position, Line, Color.green);
                        Vector3 PPos = PPosOr + transform.position;

                        PPos += Mathf.Sin((Line.magnitude - PPosOr.magnitude)) * AnimHeight * (PPosOr.magnitude - Line.magnitude) * (AnimTime - AnimTimer) * (quat * Vector3.up);

                        if (Line.magnitude - PPosOr.magnitude == 0) Debug.Log("wefg");
                        if (PPosOr.magnitude <= length * AnimTimer)
                        {
                            lastPos = PPos;
                            renderer.SetPosition(i, PPos);
                        }
                        else
                        {
                            renderer.SetPosition(i, lastPos);
                        }
                    }
                }
                else
                {
                    renderer.positionCount = 2;
                    renderer.SetPosition(1, hookPoint.transform.position);
                    renderer.SetPosition(0, transform.position);


                    Hips.LookAt(hookPoint);

                    SpringJoint joint = GetComponent<SpringJoint>();
                    if (joint.massScale == .000000001f)
                    {
                        joint.connectedBody = hookPoint.GetComponent<Rigidbody>();
                        joint.connectedAnchor =Vector3.zero;
                        joint.spring = 1.5f;
                        joint.damper =1.5f;
                        joint.massScale = 4.5f;
                        joint.maxDistance = lastDis * 0.8f;
                        joint.minDistance = lastDis * 0.2f;
                        joint.massScale = 10f;
                        joint.connectedMassScale = 4.5f;
                        PManager.PInfo.Anim.SetBool("isSwinging", true);
                       PManager.PInfo.Rb.AddForce(transform.forward * 500 );
                        PManager.PInfo.Rb.AddForce(-transform.up * 500 );
                    }
                }
            }
            else
            {
                LineRenderer renderer = PManager.PInfo.LineRenderer;
                renderer.positionCount = 0;

                SpringJoint joint = GetComponent<SpringJoint>();
                if (joint.massScale != .000000001f)
                {
                    joint.massScale = .000000001f;
                    joint.connectedBody = null;
                    PManager.PInfo.Anim.SetBool("isSwinging", false);
                }
            }
            if ((hookPoint != null && !PManager.Fire3) || PManager.OnGround)
            {
                // CameraController.CController.RemoveFromFollowing(hookPoint);
                AnimTimer = 0;
                hookPoint = null;
                PManager.AllowedMoves = 1;
                if (PManager.OnGround) PManager.AllowedMoves = 0;
            }
        }

        public override bool Allowed(int allowedMoves)
        {
            return !PManager.OnGround;
        }
    }
}