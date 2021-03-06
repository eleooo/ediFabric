﻿//---------------------------------------------------------------------
// This file is part of ediFabric
//
// Copyright (c) ediFabric. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace EdiFabric.Framework.Envelopes.X12
{
    /// <summary>
    /// X12 interchange
    /// </summary>
    [XmlRoot("INTERCHANGE", Namespace = Namespaces.X12)]
    public class Interchange
    {
        /// <summary>
        /// Interchange header
        /// </summary>
        [XmlElement("S_ISA", Order = 0)]
        public S_ISA Isa { get; set; }

        /// <summary>
        /// Groups
        /// </summary>
        [XmlArray("GROUPS", Order = 1)]
        [XmlArrayItem("GROUP", IsNullable = false)]
        public List<Group> Groups { get; set; }

        /// <summary>
        /// Interchange trailer
        /// </summary>
        [XmlElement("S_IEA", Order = 2)]
        public S_IEA Iea { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interchange"/> class.
        /// </summary>
        public Interchange()
        {
            Groups = new List<Group>();
        }

        /// <summary>
        /// Factory to initialize a new instance of the <see cref="Interchange"/> class.
        /// </summary>
        /// <param name="ediStream">The edi stream.</param>
        /// <returns>
        /// The interchange instance.
        /// </returns>
        public static Interchange LoadFrom(Stream ediStream)
        {
            if (ediStream == null) throw new ArgumentNullException("ediStream");

            var x12Lexer = new FromEdiLexer(ediStream.ToEdiString());
            x12Lexer.Analyze();

            // X12 headers are positional and the blank spaces need to be preserved
            x12Lexer.Result.Isa.D_745_2 = x12Lexer.Result.Isa.D_745_2.PadRight(10, ' ');
            x12Lexer.Result.Isa.D_747_4 = x12Lexer.Result.Isa.D_747_4.PadRight(10, ' ');

            return x12Lexer.Result;
        }

        /// <summary>
        /// Factory to initialize a new instance of the <see cref="Interchange"/> class.
        /// </summary>
        /// <param name="ediElement">
        /// The edi xml.
        /// </param>
        /// <returns>
        /// The interchange instance.
        /// </returns>
        public static Interchange LoadFrom(XElement ediElement)
        {
            if (ediElement == null) throw new ArgumentNullException("ediElement");

            var interchange = ediElement.Deserialize<Interchange>();

            // X12 headers are positional and the blank spaces need to be preserved
            interchange.Isa.D_745_2 = interchange.Isa.D_745_2.PadRight(10, ' ');
            interchange.Isa.D_747_4 = interchange.Isa.D_747_4.PadRight(10, ' ');

            return interchange;
        }

        /// <summary>
        /// Converts the interchange to edi message.
        /// </summary>
        /// <param name="context">
        /// The interchange context.
        /// This sets the non format default separators.
        /// </param>
        /// <returns>
        /// The edi message.
        /// </returns>
        public List<string> ToEdi(InterchangeContext context = null)
        {
            var x12Lexer = new ToEdiLexer(EdiHelper.Serialize(this), context);
            x12Lexer.Analyze();

            return x12Lexer.Result;
        }

        /// <summary>
        /// Serialize the interchange into xml
        /// </summary>
        /// <returns>
        /// The serialized xml.
        /// </returns>
        public XElement Serialize()
        {
            return EdiHelper.Serialize(this);
        }
    }
}
