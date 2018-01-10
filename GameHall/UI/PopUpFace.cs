using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MoleMole
{
    /// <summary>
    /// 弹出确认框点击确定回调，请实现确认取消
    /// </summary>
    interface i_CallBack
    {
        void MakeSureCallBack();
        void CancelCallBack();
    }
    /// <summary>
    /// 弹出框类型 确认+取消 仅确认 仅取消
    /// </summary>
    public enum POPTYPE { NORMAL, OK, CANCEL };
    class PopUpFaceContext : BaseContext
    {
        public POPTYPE type = POPTYPE.NORMAL;
        public string text;
        public i_CallBack callBackFace;
        public PopUpFaceContext()
            : base(UIType.NextMenu)
        {

        }
        public PopUpFaceContext(POPTYPE type,string text, i_CallBack callBack) :base(UIType.PopUpFace)
        {
            this.type = type;
            this.text = text;
            callBackFace = callBack;
        }
    }

    class PopUpFace : AnimateView
    {
        public Text text;
        public GameObject okandcancel;
        public GameObject onlycancel;
        public GameObject onlyok;

        public i_CallBack makesure=null;
        public override void OnEnter(BaseContext context)
        {
            base.OnEnter(context);
            PopUpFaceContext pContext = (PopUpFaceContext)context;
            text.text = pContext.text;
            switch (pContext.type)
            {
                case POPTYPE.NORMAL:
                    okandcancel.SetActive(true);
                    onlycancel.SetActive(false);
                    onlyok.SetActive(false);
                    break;
                case POPTYPE.CANCEL:
                    okandcancel.SetActive(false);
                    onlycancel.SetActive(true);
                    onlyok.SetActive(false);
                    break;
                case POPTYPE.OK:
                    okandcancel.SetActive(false);
                    onlycancel.SetActive(false);
                    onlyok.SetActive(true);
                    break;
            }
            makesure = pContext.callBackFace;
        }

        public override void OnExit(BaseContext context)
        {
            base.OnExit(context);
        }

        /// <summary>
        /// 点击确认
        /// </summary>
        public void OnMakeSure()
        {
            if (makesure != null)
            {
                makesure.MakeSureCallBack();
            }
        }
        /// <summary>
        /// 点击取消
        /// </summary>
        public void OnCancel()
        {
            if (makesure != null)
            {
                makesure.CancelCallBack();
            }
        }

    }
}

