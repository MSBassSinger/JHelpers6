using System;
using System.Collections.Generic;
using System.Text;

namespace Jeff.Jones.JHelpers6
{
    /// <summary>
    /// The class is sealed to prevent derived instances, which would defeat the purpose of a singleton.
    /// </summary>
    public sealed class ContextMgr : IDisposable
    {
        /// <summary>
        /// The single, thread-safe instance of the ContextMgr.
        /// </summary>
        public static readonly ContextMgr Instance = new ContextMgr();

        /// <summary>
        /// The dictionary containing the name-value pairs.
        /// </summary>
        private volatile Dictionary<String, dynamic> m_ContextValues = new Dictionary<string, dynamic>();

        /// <summary>
        /// True if dispose was called.
        /// </summary>
        private Boolean m_blnDisposeHasBeenCalled = false;


        /// <summary>
        /// Private constructor so it cannot be called externally.
        /// </summary>
        private ContextMgr()
        {
            if (m_ContextValues == null)
            {
                m_ContextValues = new Dictionary<String, dynamic>();
            }
        }

        /// <summary>
        /// Gets the dictionary of context values.
        /// The dictionary cannot be set by the calling code, but the dictioanry elements can be added, edited, or removed.
        /// </summary>
        /// <returns>Returns a reference to the dictionary.</returns>
        public Dictionary<String, dynamic> ContextValues
        {
            get
            {
                if (m_ContextValues == null)
                {
                    m_ContextValues = new Dictionary<string, dynamic>();
                }

                return m_ContextValues;
            }
        }


        #region IDisposable Implementation=========================

        /// <summary>
        /// This property is true if Dispose() has been called, false if not.
        ///
        /// The programmer does not have to check this property before calling
        /// the Dispose() method as the check is made internally and Dispose()
        /// is not executed more than once.
        /// </summary>
        public Boolean Disposing
        {
            get
            {
                return m_blnDisposeHasBeenCalled;
            }
        }  // END public Boolean Disposing

        /// <summary>
        /// Implement the IDisposable.Dispose() method
        /// Developers are supposed to call this method when done with this object.
        /// There is no guarantee when or if the GC will call it, so 
        /// the developer is responsible to.  GC does NOT clean up unmanaged 
        /// resources, so we have to clean those up, too.
        /// 
        /// </summary>
        public void Dispose()
        {
            try
            {
                // Check if Dispose has already been called 
                // Only allow the consumer to call it once with effect.
                if (!m_blnDisposeHasBeenCalled)
                {
                    // Call the overridden Dispose method that contains common cleanup code
                    // Pass true to indicate that it is called from Dispose
                    Dispose(true);

                    // Prevent subsequent finalization of this object. Subsequent finalization 
                    // is not needed because managed and unmanaged resources have been 
                    // explicitly released
                    GC.SuppressFinalize(this);
                }
            }

            catch 
            {

            }  // END Catch

        }  // END public new void Dispose()

        /// <summary>
        /// Explicit Finalize method.  The GC calls Finalize, if it is called.
        /// There are times when the GC will fail to call Finalize, which is why it is up to 
        /// the developer to call Dispose() from the consumer object.
        /// </summary>
        ~ContextMgr()
        {
            // Call Dispose indicating that this is not coming from the public
            // dispose method.
            Dispose(false);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="pDisposing">true if managed resources should be disposed; otherwise, false.</param>
        public void Dispose(Boolean pDisposing)
        {

            try
            {
                if (!m_blnDisposeHasBeenCalled)
                {
                    if (pDisposing)
                    {
                        // Here we dispose and clean up the unmanaged objects and managed object we created in code
                        // that are not in the IContainer child object of this object.
                        // Unmanaged objects do not have a Dispose() method, so we just set them to null
                        // to release the reference.  For managed objects, we call their respective Dispose()
                        // methods, if they have them, and then release the reference.
                        // if (m_objComputers != null)
                        //     {
                        //     m_objComputers = null;
                        //     }

 
                        if (m_ContextValues != null)
                        {

                            m_ContextValues.Clear();

                            m_ContextValues = null;
                        }


                        // If the base object for this instance has a Dispose() method, call it.
                        //base.Dispose();
                    }

                    // Set the flag that Dispose has been called and executed.
                    m_blnDisposeHasBeenCalled = true;
                }

            }

            catch 
            {
 
            }  // END Catch
        }

        #endregion IDisposable Implementation======================


    }
}
