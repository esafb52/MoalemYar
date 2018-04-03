/****************************** ghost1372.github.io ******************************\
*	Module Name:	User.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 4, 1, 05:31 ب.ظ
*
***********************************************************************************/

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoalemYar.DataClass.Tables
{
    [Table("Schools")]
    public class School
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string SchoolName { get; set; }

        [Required]
        public string Year { get; set; }

        [Required]
        public string Base { get; set; }

        public string Admin { get; set; }
    }
}