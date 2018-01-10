using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 *	
 *  
 *
 *	by GWB
 *
 */

namespace MoleMole
{
    class MainUIContext : BaseContext
    {
        public MainUIContext() : base(UIType.MainUI)
        {
           // ViewType = UIType.MainUI;
        }
    }

    class MainUIView : AnimateView,i_CallBack
    {

        public int m_Text;
        public bool b_isClick;
        /*[SerializeField]
        private Button _buttonHighScore;
        [SerializeField]
        private Button _buttonOption;
        */
        public MainUIView()
        {
            //int ii = 1;
        }

        public void Start()
        {
            m_Text = 1;
        }
		//------------------------此处向下的代码无用-----------------------------
        public override void OnEnter(BaseContext context)
        {
            b_isClick = false;
            base.OnEnter(context);
 
        }

        public override void OnExit(BaseContext context)
        {
            base.OnExit(context);
        }

        public override void OnPause(BaseContext context)
        {
            _animator.SetTrigger("OnExit");
        }

        public override void OnResume(BaseContext context)
        {
            b_isClick = false;
            _animator.SetTrigger("OnEnter");
        }

        public void OptionCallBack()
        {
            Singleton<ContextManager>.Instance.Push(new OptionMenuContext());
        }

        public void HighScoreCallBack()
        {
            //RaceSceneControl.firstUISceneId = UiSceneUICamera.UISceneId.Id_UIGameStart;
            //Application.LoadLevelAsync(SystemCommand.FirstSceneName);
            //((IUniGameBootFace1)gameBootFace).SetStartLevelLoad(Application.LoadLevelAsync(SystemCommand.FirstSceneName));
            Singleton<ContextManager>.Instance.Push(new PopUpFaceContext(POPTYPE.NORMAL, "其实我很怀旧！", (i_CallBack)this));
            //Singleton<ContextManager>.Instance.Push(new HighScoreContext());
        }
        public void OnQinRenClick()
        {

        }
        public void MakeSureCallBack()
        {
            Singleton<ContextManager>.Instance.Pop();
        }
        public void CancelCallBack()
        {
            Singleton<ContextManager>.Instance.Pop();
        }
    }
}