
/****************************** ghost1372.github.io ******************************\
*	Module Name:	201806011337547_non.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 1, 06:07 È.Ù
*	
***********************************************************************************/
namespace MoalemYar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class non : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "ss");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "ss", c => c.String(maxLength: 2147483647));
        }
    }
}
