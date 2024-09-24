namespace UnityEngine.UI.Extensions.Examples
{
    public class AnimateEffects : MonoBehaviour
    {
        private float _letterSpacingMax = 10;
        private float _letterSpacingMin = -10;
        private float _curvedTextMax = 0.05f;
        private float _letterSpacingModifier = 0.1f;
        private float _curvedTextMin = -0.05f;
        private float _gradient2Max = 1;
        private float _gradient2Min = -1;
        private float _curvedTextModifier = 0.001f;
        private float _gradient2Modifier = 0.01f;
        private Transform _cylinderTextRT;
        private Vector3 _cylinderRotation = new Vector3(0, 1, 0);
        private float _saUIMMax = 1;
        private float _saUIMMin = 0;
        private float _saUIMModifier = 0.01f;

        public LetterSpacing letterSpacing;
        public CurvedText curvedText;
        public Gradient2 gradient2;
        public CylinderText cylinderText;
        public SoftMaskScript saUIM;
        // Use this for initialization
        void Start()
        {
            _cylinderTextRT = cylinderText.GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            letterSpacing.spacing += _letterSpacingModifier;
            if (letterSpacing.spacing > _letterSpacingMax || letterSpacing.spacing < _letterSpacingMin)
            {
                _letterSpacingModifier = -_letterSpacingModifier;
            }
            curvedText.CurveMultiplier += _curvedTextModifier;
            if (curvedText.CurveMultiplier > _curvedTextMax || curvedText.CurveMultiplier < _curvedTextMin)
            {
                _curvedTextModifier = -_curvedTextModifier;
            }
            gradient2.Offset += _gradient2Modifier;
            if (gradient2.Offset > _gradient2Max || gradient2.Offset < _gradient2Min)
            {
                _gradient2Modifier = -_gradient2Modifier;
            }

            _cylinderTextRT.Rotate(_cylinderRotation);

            saUIM.CutOff += _saUIMModifier;
            if (saUIM.CutOff > _saUIMMax || saUIM.CutOff < _saUIMMin)
            {
                _saUIMModifier = -_saUIMModifier;
            }

        }
    }
}