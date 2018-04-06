/****************************** ghost1372.github.io ******************************\
*	Module Name:	Student.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 28, 12:01 ب.ظ
*
***********************************************************************************/

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoalemYar.DataClass.Tables
{
    [Table("Students")]
    public class Student
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long BaseId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LName { get; set; }

        [Required]
        public string FName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public byte[] Image { get; set; }
    }
}