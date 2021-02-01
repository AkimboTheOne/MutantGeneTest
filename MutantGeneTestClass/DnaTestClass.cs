using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MutantGeneTestClass
{
    
    /// <summary>
    /// clase para muestras de cadenas dna
    /// </summary>
    public class DnaTestClass : IDisposable
    {


        /// <summary>
        /// secuencias dna requeridas para identificar un mutante
        /// </summary>
        static int _minDnaChainSequencesToBeMutant = 2;
        /// <summary>
        /// número de repeticiones requeridas para identificar una secuencia dna mutante
        /// </summary>
        static int _mutantChainTargetSequence = 4;
        /// <summary>
        /// máxima longitud de secuencias dna
        /// </summary>
        static int _dnaMaxSampleLenght = 100;
        /// <summary>
        /// bases nitrogenadas permitidas en dna
        /// </summary>
        static char[] _dnaAllowedChains = new char[] { 'A', 'T', 'C', 'G' };


        // excepción
        string _error = "";
        public string Error { get { return _error; } }


        string[] _dna;
        /// <summary>
        /// cadena dna
        /// </summary>
        public string[] DNA { get { return !(_dna is null) ? _dna : new string[0]; }  }


        bool _isValid = false;
        /// <summary>
        /// validez de la cadena dna
        /// </summary>
        public bool IsValid { get { return _isValid; } }

        
        int _length = 0;
        /// <summary>
        /// longitud NxN de la cadena dna
        /// </summary>
        public int Length { get { return _length; } }

        /// <summary>
        /// firma de cadena dna
        /// </summary>
        public string DNASignature { get { return _isValid ? _DNASignature() : ""; } }
        string _DNASignature()
        {
            string _s = "";
            foreach (var item in _dna)
                _s += item;
            return _s;
        }

        /// <summary>
        /// valida y carga una cadena dna en la muestra
        /// </summary>
        /// <param name="dna"></param>
        /// <returns></returns>
        bool LoadDnaChain (string[] dna)
        {
            try
            {
                
                // GUARDIANES

                // null guard
                if (dna is null) throw new NullReferenceException("NO SE HA SUMINISTRADO UNA CADENA DNA");
                // debe tener elementos
                if (dna.Length == 0) throw new ArgumentOutOfRangeException(nameof(dna), "LA CADENA DNA NO TIENE ELEMENTOS");
                // NxN debe ser de al menos 5x5 pra encontrar "más de una secuencia de cuatro letras iguales" (*** sin compartir posiciones con otras cadenas)
                // NxN no puede ser mayor a 100x100 (guardian de memoria-recursos)
                if ((dna[0].Length < (_mutantChainTargetSequence + 1) || dna[0].Length > _dnaMaxSampleLenght) || (dna.Length < (_mutantChainTargetSequence + 1) || dna.Length > _dnaMaxSampleLenght)) throw new ArgumentOutOfRangeException(nameof(dna), "LA CADENA DNA NO TIENE LAS SECUENCIAS MÍNIMAS PARA PODER SER ANALIZADA (MIN:" + _mutantChainTargetSequence + ", MÁX:" + _dnaMaxSampleLenght + ")");
                // NxN se debe proyectar como cuadrada tomando como "referencia" el ancho de la primera línea
                if (dna.Length < dna[0].Length) throw new ArgumentOutOfRangeException(nameof(dna), "LA CADENA DNA NO ES CUADRÁTICA NxN (MIN:" +_mutantChainTargetSequence+", MÁX:"+_dnaMaxSampleLenght+")");
                // NxN debe ser perfectamente cuadrada tomando como "referencia" el ancho de la primera línea
                foreach (var line in dna)
                {
                    if (line.Length != dna[0].Length) throw new ArgumentOutOfRangeException(nameof(dna), "LA CADENA DNA NO ES CUADRÁTICA NxN (MIN:" + _mutantChainTargetSequence + ", MÁX:" + _dnaMaxSampleLenght + ")");
                    // sólo cadenas dna válidas
                    foreach (var c in line.ToCharArray())
                        if (!(_dnaAllowedChains.ToList().Contains(c))) throw new ArgumentOutOfRangeException(nameof(dna), "LA CADENA DNA PRESENTA BASES NITROGENADAS NO RECONOCIDAS (SÓLO A, T, C, G)");
                }

                // es una cadena dna válida
                _dna = dna;
                _isValid = true;
                _length = dna[0].Length;

                return true; // SALIDA

            }
            catch (Exception ex)
            {
                _error = ex.Message;
                return false;
            }
         }
        /// <summary>
        /// valida y carga una cadena dna en la muestra
        /// </summary>
        /// <param name="dna"></param>
        /// <returns></returns>
        bool LoadDnaChain (DnaClass dna)
        {
            return LoadDnaChain(dna.dna);
        }


        // mapa dna en cadena
        bool[,] _mutantChainsMap = new bool[_dnaMaxSampleLenght, _dnaMaxSampleLenght];
        bool CheckMutantChainOnMap (int x, int y)
        {
            return _mutantChainsMap[x, y];
        }
        void MarkMutantChainOnMap(int x, int y)
        {
            _mutantChainsMap[x, y] = true;
        }


        // secuencias detectadas
        int _dnaMutantChainSequencesDetected = 0;
        bool _dnaMutantChainSequencesChecked = false;
        bool _isMutant = false;
        public bool IsMutant()
        {
            try
            {
                // GUARDIANES

                // no es una cadena dna válida
                if (!_isValid) throw new MutantValidatonException();
                // ya fué evaulado como mutante
               if (_dnaMutantChainSequencesChecked) return _isMutant;

                // LÓGICA GUARDIAN
                var _travel  = 0;
                for (int y = 0; y <= this.Length - 1; y++)
                {
                    if (_dnaMutantChainSequencesDetected >= _minDnaChainSequencesToBeMutant) break; // interrupción
                    var line = _dna[y];
                    for (int x = 0; x <= this.Length - 1; x++)
                    {
                        if (_dnaMutantChainSequencesDetected >= _minDnaChainSequencesToBeMutant) break; // interrupción

                        ValidateMutantChainHorizontal(x, y);
                        ValidateMutantChainVertical(x, y);
                        ValidateMutantChainObliqueLR(x, y);
                        ValidateMutantChainObliqueRL(x, y);

                        _travel++;
                    }

                }
                Debug.WriteLine("EVALUACIONES: " + _travel);
                // es humano
                if (_dnaMutantChainSequencesDetected < _minDnaChainSequencesToBeMutant) throw new MutantValidatonException("NO ES UN MUNTANTE");

                _dnaMutantChainSequencesChecked = true;
                _isMutant = true;
                return true;
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                _dnaMutantChainSequencesChecked = true;
                _isMutant = false;
                return false;
            }
        }


        // validaciones secuencias de mutación
        bool ValidateMutantChainHorizontal (int x, int y)
        {
            try
            {
                // GUARDIANES

                // la casilla inicial no se ha reconocido como parte de una cadena mutante
                if (CheckMutantChainOnMap(x,y)) throw new MutantValidatonException();
                // hay espacio para el recorrido (secuencia)
                if ((x - 1 + _mutantChainTargetSequence) > this.Length) throw new MutantValidatonException();

                // LÓGICA GUARDIAN
                for (int i = 0; i < _mutantChainTargetSequence; i++)
                {
                    if (CheckMutantChainOnMap(x + i, y) || _dna[y].Substring(x + i, 1) != _dna[y].Substring(x, 1)) throw new MutantValidatonException();
                }
                
                Debug.WriteLine("CADENA HORIZONTAL X: " + x + " Y: " + y);
                MarkMutantChainHorizontal(x, y);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        bool ValidateMutantChainVertical (int x, int y)
        {
            try
            {

                // la casilla inicial no se ha reconocido como parte de una cadena mutante
                if (CheckMutantChainOnMap(x, y)) throw new MutantValidatonException();
                // hay espacio para el recorrido (secuencia)
                if ((y - 1 + _mutantChainTargetSequence) > this.Length) throw new MutantValidatonException();

                // LÓGICA GUARDIAN
                for (int i = 0; i < _mutantChainTargetSequence; i++)
                {
                    if (CheckMutantChainOnMap(x, y + i) || _dna[y + i].Substring(x, 1) != _dna[y].Substring(x, 1)) throw new MutantValidatonException();
                }
                
                Debug.WriteLine("CADENA VERTICAL X: " + x + " Y: " + y);
                MarkMutantChainVertical(x, y);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        bool ValidateMutantChainObliqueRL (int x, int y)
        {
            try
            {

                // la casilla inicial no se ha reconocido como parte de una cadena mutante
                if (CheckMutantChainOnMap(x, y)) throw new MutantValidatonException();
                // hay espacio para el recorrido (secuencia)
                if ((x + 1 - _mutantChainTargetSequence) >= 0) throw new MutantValidatonException();
                if ((y - 1 + _mutantChainTargetSequence) > this.Length) throw new MutantValidatonException();

                // LÓGICA GUARDIAN
                for (int i = 0; i < _mutantChainTargetSequence; i++)
                {
                    if (CheckMutantChainOnMap(x - i, y + i) || _dna[y + i].Substring(x - i, 1) != _dna[y].Substring(x, 1)) throw new MutantValidatonException();
                }

                Debug.WriteLine("CADENA OBLICUA DER-IZQ X: " + x + " Y: " + y);
                MarkMutantChainObliqueRL(x, y);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        bool ValidateMutantChainObliqueLR (int x, int y)
        {
            try
            {

                // la casilla inicial no se ha reconocido como parte de una cadena mutante
                if (CheckMutantChainOnMap(x, y)) throw new MutantValidatonException();
                // hay espacio para el recorrido (secuencia)
                if ((x - 1 + _mutantChainTargetSequence) > this.Length) throw new MutantValidatonException();
                if ((y - 1 + _mutantChainTargetSequence) > this.Length) throw new MutantValidatonException();

                // LÓGICA GUARDIAN
                for (int i = 0; i < _mutantChainTargetSequence; i++)
                {
                    if (CheckMutantChainOnMap(x + i, y + i) || _dna[y + i].Substring(x + i, 1) != _dna[y].Substring(x, 1)) throw new MutantValidatonException();
                }

                Debug.WriteLine("CADENA OBLICUA IZQ-DER X: " + x + " Y: " + y);
                MarkMutantChainObliqueLR(x, y);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        
        // marcar secuencias de mutación
        void MarkMutantChainHorizontal(int x, int y)
        {
            for (int i = 0; i < _mutantChainTargetSequence; i++)
            {
                MarkMutantChainOnMap(x + i, y);
            }
            _dnaMutantChainSequencesDetected++;
        }
        void MarkMutantChainVertical(int x, int y)
        {
            for (int i = 0; i < _mutantChainTargetSequence; i++)
            {
                MarkMutantChainOnMap(x, y + i);
            }
            _dnaMutantChainSequencesDetected++;
        }
        void MarkMutantChainObliqueRL(int x, int y)
        {
            for (int i = 0; i < _mutantChainTargetSequence; i++)
            {
                MarkMutantChainOnMap(x - i, y + i);
            }
            _dnaMutantChainSequencesDetected++;
        }
        void MarkMutantChainObliqueLR(int x, int y)
        {
            for (int i = 0; i < _mutantChainTargetSequence; i++)
            {
                MarkMutantChainOnMap(x + i, y + i);
            }
            _dnaMutantChainSequencesDetected++;
        }


        public void Dispose()
        {
            if (!(_dna is null)) Array.Clear(_dna, 0, _dna.Length);
            
            _length = 0;
            _isValid = false;

            _isMutant = false;
            _dnaMutantChainSequencesDetected = 0;
            _dnaMutantChainSequencesChecked = false;
        }


        // CONSTRUCTORES
        public DnaTestClass(string[] dna)
        {
            LoadDnaChain(dna);
        }
        public DnaTestClass(DnaClass dna)
        {
            LoadDnaChain(dna);
        }


    }

    [Serializable]
    public class MutantValidatonException : ApplicationException
    {
        public MutantValidatonException() { }
        public MutantValidatonException(string message) : base(message) { }
        public MutantValidatonException(string message, Exception inner) : base(message, inner) { }
        protected MutantValidatonException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
