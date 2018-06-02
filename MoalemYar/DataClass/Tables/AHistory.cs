
/****************************** ghost1372.github.io ******************************\
*	Module Name:	AHistory.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 2, 08:11 ب.ظ
*	
***********************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoalemYar.DataClass.Tables
{
    [Table("AHistory")]
    public class AHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long UserId { get; set; }
        public string DatePass { get; set; }
        public string GroupName { get; set; }
        public int TrueItem { get; set; }
        public int FalseItem { get; set; }
        public int NoneItem { get; set; }
    }
}
