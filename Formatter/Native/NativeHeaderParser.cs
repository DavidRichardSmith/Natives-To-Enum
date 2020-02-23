using System;
using System.Collections.Generic;
using System.IO;

namespace Formatter.Native
{
    /// <summary>
    /// Represents a single native.
    /// </summary>
    public struct Native
    {
        /// <summary>
        /// The name of the native.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The hash of the native.
        /// </summary>
        public string Hash { get; set; }
	}

    /// <summary>
    /// Parser responsible for extracting native data inside of a natives.h file.
    /// </summary>
    public class NativeHeaderParser
    {
        #region PUBLIC

		/// <summary>
		/// Construct <see cref="NativeHeaderParser"/> with path to natives.h file.
		/// </summary>
		/// <param name="fileName"></param>
		public NativeHeaderParser(string fileName)
        {
            _fileName = fileName;
            _natives  = new List<Native>();
        }

        /// <summary>
        /// Read all header data into virtual memory.
        /// </summary>
        /// <param name="lineBufferSize">Pre allocated line buffer size, this should at least be the number of lines in the header file.</param>
        public void Initialize(int lineBufferSize = 5354)
        {
            _fileLines = new List<string>();
			using(var reader = File.OpenText(_fileName))
            {
				while(!reader.EndOfStream)
                {
                    _fileLines.Add(reader.ReadLine());
                }
            }
        }

		/// <summary>
		/// Start parsing the header data.
		/// </summary>
		public void Start()
        {
			foreach(var line in _fileLines)
                processLine(line);
        }

		/// <summary>
		/// Retrieve results of the parse.
		/// </summary>
		/// <returns></returns>
		public List<Native> GetResults()
        {
            return _natives;
        }

        /// <summary>
        /// Retrieve the length of the longest native name.
        /// </summary>
        /// <returns></returns>
        public int GetLongestNativeName()
        {
            return _longestNativeName;
        }

        #endregion

        #region PRIVATE

        /// <summary>
        /// Process raw header file line.
        /// </summary>
        /// <param name="line"></param>
		private void processLine(string line)
        {
            // can't process lines less then 7.
            if (line.Length < 7)
                return;

            // if we found a line containing a native declaration.
            if (line.Substring(0, 7) == "\tstatic")
            {
                var native = processDeclaration(line);

                if (native.Name.Length > _longestNativeName)
                    _longestNativeName = native.Name.Length;

                _natives.Add(native);
            }
        }

        /// <summary>
        /// Process line containing native declaration.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private Native processDeclaration(string line)
        {
            Native native = new Native();

            native.Name = getNativeName(line);
            native.Hash = getNativeHash(line);

            return native;
        }

		/// <summary>
		/// Retrieve the name of the native by line content.
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		private string getNativeName(string line)
        {
            // retrieve start of native name by finding the first space after the native return value type.
			int index  = line.Substring(8, line.Length - 8).IndexOf(" ") + 9;

            // retrieve the length by finding the start of the function arguments.
            int length = line.Substring(index, line.Length - index).IndexOf("(");

            return line.Substring(index, length);
        }

		/// <summary>
		/// Retrieve the hash of the native by line content.
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		private string getNativeHash(string line)
        {
            // retrieve start of native hash by finding the ending template type mark.
            int index  = line.IndexOf(">") + 2;

            // native hashes are always 18 characters long.
            int length = 18;

            return line.Substring(index, length);
        }

        #endregion

        private string       _fileName;
        private List<string> _fileLines;
        private List<Native> _natives;
        private int          _longestNativeName;
    }
}
