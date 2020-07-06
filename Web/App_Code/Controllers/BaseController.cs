using Toledo.Core;

namespace Toledo.Controllers
{

    public abstract class BaseController
    {

        protected ContextWrapper Context;

        #region CONSTRUCTORES

        protected BaseController(ContextWrapper contextWrraper)
        {
            Context = contextWrraper;
        }

        #endregion

    }

}