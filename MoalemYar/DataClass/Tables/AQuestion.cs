/****************************** ghost1372.github.io ******************************\
*	Module Name:	AQuestion.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 1, 07:03 ب.ظ
*
***********************************************************************************/

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoalemYar.DataClass.Tables
{
    [Table("AQuestions")]
    public class AQuestion
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long GroupId { get; set; }
        public string Class { get; set; }
        public string QuestionText { get; set; }
        public string Case1 { get; set; }
        public string Case2 { get; set; }
        public string Case3 { get; set; }
        public string Case4 { get; set; }
        public int Answer { get; set; }
        public string Date { get; set; }
    }
}