﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MoleMole
{
    class NextMenuContext : BaseContext
    {
        public NextMenuContext()
            : base(UIType.NextMenu)
        {

        }
    }

    class NextMenuView : AnimateView
    {
        public override void OnEnter(BaseContext context)
        {
            base.OnEnter(context);
        }

        public override void OnExit(BaseContext context)
        {
            base.OnExit(context);
        }

        public void BackCallBack()
        {
            Singleton<ContextManager>.Instance.Pop();
        }

        public void ChangeLangCallBack()
        {
            if (Singleton<Localization>.Instance.Language == Localization.CHINESE)
            {
                Singleton<Localization>.Instance.Language = Localization.ENGLISH;
            }
            else
            {
                Singleton<Localization>.Instance.Language = Localization.CHINESE;
            }
        }
    }
}

