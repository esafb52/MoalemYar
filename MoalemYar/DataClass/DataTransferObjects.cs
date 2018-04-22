/****************************** ghost1372.github.io ******************************\
*	Module Name:	DataTransferObjects.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 4, 5, 05:06 ب.ظ
*
***********************************************************************************/

namespace MoalemYar.DataClass
{
    public class DataTransferObjects
    {
        public class SchoolsStudentsJointDto
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string LName { get; set; }
            public string FName { get; set; }
            public string Gender { get; set; }
            public long BaseId { get; set; }
            public byte[] Image { get; set; }
            public string Base { get; set; }
        }

        public class StudentsDto
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string LName { get; set; }
            public string FName { get; set; }
            public long BaseId { get; set; }
        }

    }
}