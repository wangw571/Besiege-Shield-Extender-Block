using System;
using System.Collections;
using System.Collections.Generic;
using spaar.ModLoader;
using TheGuysYouDespise;
using UnityEngine;

namespace Blocks
{
    public class TrackingComputerMod : BlockMod
    {
        public override Version Version { get { return new Version("0.10"); } }
        public override string Name { get { return "Shield_Extender_Mod"; } }
        public override string DisplayName { get { return "Shield Extender Block Mod"; } }
        public override string BesiegeVersion { get { return "v0.3"; } }
        public override string Author { get { return "覅是"; } }
        protected Block TurretBlock = new Block()
            ///模块ID
            .ID(530)

            ///模块名称
            .BlockName("Shield Extender")

            ///模型信息
            .Obj(new List<Obj> { new Obj("Shield Extender.obj", //Obj
                                         "Shield Extender.png", //贴图 
                                         new VisualOffset(new Vector3(1f, 1f, 1f), //Scale
                                                          new Vector3(0f, 0f, 0f), //Position
                                                          new Vector3(-90f, 0f, 0f)))//Rotation
            })

            ///在UI下方的选模块时的模样
            .IconOffset(new Icon(new Vector3(1f, 1f, 1f),  //Scale
                                 new Vector3(-0.11f, -0.13f, 0.00f),  //Position
                                 new Vector3(350f, 150f, 250f))) //Rotation

            ///没啥好说的。
            .Components(new Type[] {
                                    typeof(TrackingComputer),
            })

            ///给搜索用的关键词
            .Properties(new BlockProperties().SearchKeywords(new string[] {
                                                             "Shield",
                                                             "防御",
                                                             "Defensive"
                                             }).Burnable(3)
            )
            ///质量
            .Mass(2f)

            ///是否显示碰撞器（在公开你的模块的时候记得写false）
            .ShowCollider(false)

            ///碰撞器
            .CompoundCollider(new List<ColliderComposite> {
                ColliderComposite.Box(new Vector3(1f, 1f, 1.2f), new Vector3(0f, 0f, 0.6f), new Vector3(0f, 0f, 0f)),
            })

            ///载入资源
            .NeededResources(new List<NeededResource> {
                                new NeededResource(ResourceType.Mesh,"turret.obj")
            })

            ///连接点
            .AddingPoints(new List<AddingPoint> {
                               (AddingPoint)new BasePoint(true, true)         //底部连接点。第一个是指你能不能将其他模块安在该模块底部。第二个是指这个点是否是在开局时粘连其他链接点
                                                .Motionable(true,true,true) //底点在X，Y，Z轴上是否是能够活动的。
                                                .SetStickyRadius(0.5f),        //粘连距离
            });


        public override void OnLoad()
        {
            LoadBlock(TurretBlock);//加载该模块
        }
        public override void OnUnload() { }
    }


    public class TrackingComputer : BlockScript
    {
        protected GameObject Shield;

        /*public override void SafeAwake()
        {
            Key1 = AddKey("Lock On", //按键信息
                                 "Locked",           //名字
                                 KeyCode.T);       //默认按键

            Key2 = AddKey("Release Lock", //按键信息2
                                 "RLocked",           //名字
                                 KeyCode.Slash);       //默认按键
            List<string> chaos = new List<String> { "Turret Tracking \nComputer", "Missile Guidance \nComputer", "Camera Tracking \nComputer" };
            模式 = AddMenu("Mode", 0, chaos);

            炮力 = AddSlider("Cannon Slider",       //滑条信息
                                    "CanonPower",       //名字
                                    1f,            //默认值
                                    0f,          //最小值
                                    2f);           //最大值

            精度 = AddSlider("Precision",       //滑条信息
                                    "Precision",       //名字
                                    0.5f,            //默认值
                                    0.01f,          //最小值
                                    10f);           //最大值

            计算间隔 = AddSlider("Calculation per second",       //滑条信息 
                                    "CalculationPerSecond",       //名字
                                    100f,            //默认值
                                    1f,          //最小值
                                    100f);           //最大值

            /*不聪明模式 = AddToggle("Disable Smart Attack",   //toggle信息
                                       "NoSA",       //名字
                                       false);             //默认状态*/
        /* }*/

        protected virtual IEnumerator UpdateMapper()
        {
            if (BlockMapper.CurrentInstance == null)
                yield break;
            while (Input.GetMouseButton(0))
                yield return null;
            BlockMapper.CurrentInstance.Copy();
            BlockMapper.CurrentInstance.Paste();
            yield break;
        }
        public override void OnSave(XDataHolder data)
        {
            SaveMapperValues(data);
        }
        public override void OnLoad(XDataHolder data)
        {
            LoadMapperValues(data);
            if (data.WasSimulationStarted) return;
        }

        protected override void BuildingUpdate()
        {
            
        }
        protected override void OnSimulateStart()
        {
            this.GetComponent<ConfigurableJoint>().breakForce = Mathf.Infinity;
            this.GetComponent<ConfigurableJoint>().breakTorque = Mathf.Infinity;
            Shield = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BoxCollider Scoll = Shield.GetComponent<BoxCollider>();
            Scoll.isTrigger = true;
            Shield.GetComponent<Renderer>().material = new Material(Shader.Find("Transparent/Diffuse"));
            Shield.transform.localScale = new Vector3(5, 7, 5);
            Shield.AddComponent<ShieldScript>();

        }

        protected override void OnSimulateFixedUpdate()
        {
            Shield.transform.position = this.transform.TransformPoint(0, 0, 2);
        }
        //Physics stuff
    }

    public class ShieldScript:MonoBehaviour
    {
        public float MyCapacity;
        void Start()
        {
            MyCapacity = 100;
        }
        void FixedUpdate()
        {
            ++MyCapacity;
        }
        void OnTriggerEnter(Collider coll)
        {
            coll.attachedRigidbody.velocity *= Math.Min(Mathf.Log10(coll.attachedRigidbody.velocity.sqrMagnitude + 0.0001f) / coll.attachedRigidbody.velocity.sqrMagnitude+0.001f,1);
        }
        void OnTriggerStay(Collider coll)
        {
            coll.attachedRigidbody.velocity *= Math.Min(Math.Abs( Mathf.Log10(coll.attachedRigidbody.velocity.sqrMagnitude + 0.0001f) / coll.attachedRigidbody.velocity.sqrMagnitude + 0.001f), 1);
            coll.attachedRigidbody.AddForce(this.transform.up * 15);
            if(coll.gameObject.GetComponentInParent<ExplodeOnCollideBlock>())
            {
                coll.gameObject.GetComponentInParent<ExplodeOnCollideBlock>().radius = 0;
            }
            if (coll.gameObject.GetComponentInParent<TimedRocket>())
            {
                coll.gameObject.GetComponentInParent<TimedRocket>().radius = 0;
            }
            if (coll.gameObject.GetComponentInParent<ControllableBomb>())
            {
                coll.gameObject.GetComponentInParent<ControllableBomb>().radius = 0;
            }
        }
        void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.GetComponentInParent<ExplodeOnCollideBlock>())
            {
                coll.gameObject.GetComponentInParent<ExplodeOnCollideBlock>().radius = 7;
            }
            if (coll.gameObject.GetComponentInParent<TimedRocket>())
            {
                coll.gameObject.GetComponentInParent<TimedRocket>().radius = 3;
            }
            if (coll.gameObject.GetComponentInParent<ControllableBomb>())
            {
                coll.gameObject.GetComponentInParent<ControllableBomb>().radius = 3;
            }
        }
        void FixedUpdate()
        {

        }
    }

}


