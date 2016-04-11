using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using presage;

namespace ALSProject
{
    public class PresagePredictor
    {

        protected Presage presage;
        protected String buffer;
        protected String nextWord;
        
        #region Constructors
        public PresagePredictor()
        {
            buffer = "";
            nextWord = "";
            presage = new Presage
               (
                   callback_get_past_stream,
                   callback_get_future_stream
               );
        }
        #endregion

        #region Public Methods
        public void reset()
        {
            buffer = "";
        }

        public String[] getPredictions(string inputString)
        {
            buffer = inputString;
            return presage.predict();
        }

        private string callback_get_past_stream()
        {
            return buffer;

        }

        private string callback_get_future_stream()
        {
            return nextWord;
        }
        #endregion
    }
}
