//-----------------------------------------------------------------------
// <copyright file="Game.cs" company="DarthNemesis">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>DarthNemesis</author>
// <date>2010-02-08</date>
// <summary>Common properties and methods for the entire game.</summary>
//-----------------------------------------------------------------------

namespace SagaTrans
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using DarthNemesis;
    
    /// <summary>
    /// Common properties and methods for the entire game.
    /// </summary>
    public class Game : AbstractGame
    {
        /// <summary>
        /// Initializes a new instance of the Game class.
        /// </summary>
        public Game() : base()
        {
        }
        
        /// <summary>
        /// Gets the application name.
        /// </summary>
        /// <value>The application name.</value>
        public override string Name
        {
            get
            {
                return "SagaTrans";
            }
        }
        
        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public override string Version
        {
            get
            {
                return "v1.1";
            }
        }
        
        /// <summary>
        /// Gets the application description.
        /// </summary>
        /// <value>The application description.</value>
        public override string Description
        {
            get
            {
                return "A text editing utility for the Nintendo\r\nDS game Saga 2: Hihou Densetsu.";
            }
        }
        
        /// <summary>
        /// Gets the text encoding used by script files in the project.
        /// </summary>
        /// <value>The text encoding used by script files in the project.</value>
        public override Encoding Encoding
        {
            get
            {
                return Encoding.GetEncoding("Shift_JIS");
            }
        }
        
        /// <summary>
        /// Populates the list of files in the game.
        /// </summary>
        public override void InitializeFiles()
        {
            ITextFile[] textFiles = new ITextFile[]
            {
                new Arm9File(this),
                new OverlayFile(1, this),
                new OverlayFile(2, this),
                new OverlayFile(3, this),
                new OverlayFile(4, this),
                new DNGCFile(@"chara\charamake_chara_help.bin", this),
                new DNGCFile(@"chara\charamake_group_help.bin", this),
                new DNGCFile(@"chara\chara_help.bin", this),
                new DNGCFile(@"chara\meatdic_race_help.bin", this),
                new DNGCFile(@"event\mainevent_message.bin", this),
                new DNGCFile(@"event\mainevent_title.bin", this),
                new DNGCFile(@"event\memo_message.bin", this),
                new DNGCFile(@"event\memo_title.bin", this),
                new DNGCFile(@"event\subevent_message.bin", this),
                new DNGCFile(@"event\subevent_title.bin", this),
                new DNGCFile(@"event\subevent_title_close.bin", this),
                new DNGCFile(@"event\team_name_help.bin", this),
                new DNGCFile(@"event\tutorial_main_help.bin", this),
                new DNGCFile(@"event\tutorial_sub_help.bin", this),
                new DNGCFile(@"event\tutorial_sub_msg.bin", this),
                new DNGCFile(@"event\volunteer_top_help.bin", this),
                new DNGCFile(@"field\field_help.bin", this),
                new DNGCFile(@"field\map_ability_help.bin", this),
                new DNGCFile(@"field\map_ability_help_up.bin", this),
                new DNGCFile(@"item\chain_help.bin", this),
                new DNGCFile(@"item\item_chain_word.bin", this),
                new DNGCFile(@"item\item_help.bin", this),
                new DNGCFile(@"item\legend_name.bin", this),
                new DMSTFile(@"msg\Apollon.mst", this),
                new DMSBFile(@"msg\btl_static_msg.msb", this),
                new DMSBFile(@"msg\camp_static_msg.msb", this),
                new DMSBFile(@"msg\chara_race_name.msb", this),
                new DMSBFile(@"msg\chara_type_name.msb", this),
                new DMSBFile(@"msg\fld_static_msg.msb", this),
                new DMSTFile(@"msg\Gimmick_msg.mst", this),
                new DMSTFile(@"msg\LDS_msg.mst", this),
                new DMSTFile(@"msg\MUESE1.mst", this),
                new DMSTFile(@"msg\MUESE2.mst", this),
                new DMSTFile(@"msg\MUESE3.mst", this),
                new DMSTFile(@"msg\NPC_msg.mst", this),
                new DMSTFile(@"msg\PartyChat.mst", this),
                new DMSTFile(@"msg\Sraim.mst", this),
                new DMSTFile(@"msg\SUBEVENT1.mst", this),
                new DMSTFile(@"msg\SUBEVENT2.mst", this),
                new DMSTFile(@"msg\SUBEVENT3.mst", this),
                new DMSTFile(@"msg\SUBEVENT4.mst", this),
                new DMSTFile(@"msg\SUBEVENT5.mst", this),
                new DMSTFile(@"msg\SUBEVENT6.mst", this),
                new DMSTFile(@"msg\SUBEVENT7.mst", this),
                new DMSTFile(@"msg\SUBEVENT8.mst", this),
                new DMSTFile(@"msg\SUBEVENT9.mst", this),
                new DMSTFile(@"msg\SUBEVENT10.mst", this),
                new DMSTFile(@"msg\SUBEVENT11.mst", this),
                new DMSTFile(@"msg\SUBEVENT12.mst", this),
                new DMSTFile(@"msg\System_msg.mst", this),
                new DMSTFile(@"msg\tutorial.mst", this),
                new DMSBFile(@"msg\warning_static_msg.msb", this),
                new DMSTFile(@"msg\world_000.mst", this),
                new DMSTFile(@"msg\world_001.mst", this),
                new DMSTFile(@"msg\world_002.mst", this),
                new DMSTFile(@"msg\world_003.mst", this),
                new DMSTFile(@"msg\world_004.mst", this),
                new DMSTFile(@"msg\world_005.mst", this),
                new DMSTFile(@"msg\world_006.mst", this),
                new DMSTFile(@"msg\world_007.mst", this),
                new DMSTFile(@"msg\world_008.mst", this),
                new DMSTFile(@"msg\world_009.mst", this),
                new DMSTFile(@"msg\world_010.mst", this),
                new DMSTFile(@"msg\world_011.mst", this),
                new DMSTFile(@"msg\world_012.mst", this),
                new DMSTFile(@"msg\world_013.mst", this),
                new DMSTFile(@"msg\world_014.mst", this),
                new DNGCFile(@"script\moirai_chain_help.bin", this),
                new DNGCFile(@"script\moirai_help.bin", this),
                new DNGCFile(@"script\unique_help.bin", this),
                new DCPBFile(@"script\action\act_atk.scripb", this),
                new DCPBFile(@"script\action\act_boss.scripb", this),
                new DCPBFile(@"script\action\act_evt.scripb", this),
                new DCPBFile(@"script\action\act_ext.scripb", this),
                new DCPBFile(@"script\action\act_lds.scripb", this),
                new DCPBFile(@"script\action\act_mag.scripb", this),
                new DCPBFile(@"script\action\act_mst.scripb", this),
                new DCPBFile(@"script\action\act_muse.scripb", this),
                new DCPBFile(@"script\action\act_sht.scripb", this),
                new DCPBFile(@"script\action\act_trea.scripb", this),
                new DCPBFile(@"script\action\lnk_clw.scripb", this),
                new DCPBFile(@"script\action\lnk_fag.scripb", this),
                new DCPBFile(@"script\action\lnk_mag.scripb", this),
                new DCPBFile(@"script\action\lnk_mat.scripb", this),
                new DCPBFile(@"script\action\lnk_mst.scripb", this),
                new DCPBFile(@"script\action\lnk_sht.scripb", this),
                new DCPBFile(@"script\action\lnk_swd.scripb", this),
                new DCPBFile(@"script\action\lnk_tac.scripb", this),
                new DCPBFile(@"script\action\lnk_wpn.scripb", this),
                new DCPBFile(@"script\action\tgt_dmg.scripb", this),
                new DCPBFile(@"script\action\tgt_spp.scripb", this),
                new DCPBFile(@"script\field\2ar.scripb", this),
                new DCPBFile(@"script\field\2eu.scripb", this),
                new DCPBFile(@"script\field\2me.scripb", this),
                new DCPBFile(@"script\field\2po.scripb", this),
                new DCPBFile(@"script\field\2ro.scripb", this),
                new DCPBFile(@"script\field\2rp.scripb", this),
                new DCPBFile(@"script\field\2ta.scripb", this),
                new DCPBFile(@"script\field\2ur.scripb", this),
                new DCPBFile(@"script\field\camp_atropos.scripb", this),
                new DCPBFile(@"script\field\camp_clotho.scripb", this),
                new DCPBFile(@"script\field\camp_helptalk.scripb", this),
                new DCPBFile(@"script\field\camp_lakhesis.scripb", this),
                new DCPBFile(@"script\field\e00.scripb", this),
                new DCPBFile(@"script\field\e01.scripb", this),
                new DCPBFile(@"script\field\e02.scripb", this),
                new DCPBFile(@"script\field\e03.scripb", this),
                new DCPBFile(@"script\field\eu1.scripb", this),
                new DCPBFile(@"script\field\eu2.scripb", this),
                new DCPBFile(@"script\field\eu3.scripb", this),
                new DCPBFile(@"script\field\eu4.scripb", this),
                new DCPBFile(@"script\field\eve02.scripb", this),
                new DCPBFile(@"script\field\eve03.scripb", this),
                new DCPBFile(@"script\field\k02.scripb", this),
                new DCPBFile(@"script\field\k03.scripb", this),
                new DCPBFile(@"script\field\k04.scripb", this),
                new DCPBFile(@"script\field\k05.scripb", this),
                new DCPBFile(@"script\field\k06.scripb", this),
                new DCPBFile(@"script\field\k07.scripb", this),
                new DCPBFile(@"script\field\k09.scripb", this),
                new DCPBFile(@"script\field\ka1.scripb", this),
                new DCPBFile(@"script\field\m01.scripb", this),
                new DCPBFile(@"script\field\m02.scripb", this),
                new DCPBFile(@"script\field\m03.scripb", this),
                new DCPBFile(@"script\field\m04.scripb", this),
                new DCPBFile(@"script\field\m05.scripb", this),
                new DCPBFile(@"script\field\m06.scripb", this),
                new DCPBFile(@"script\field\m07.scripb", this),
                new DCPBFile(@"script\field\m08.scripb", this),
                new DCPBFile(@"script\field\m08_01.scripb", this),
                new DCPBFile(@"script\field\m09.scripb", this),
                new DCPBFile(@"script\field\m10.scripb", this),
                new DCPBFile(@"script\field\m11.scripb", this),
                new DCPBFile(@"script\field\m12.scripb", this),
                new DCPBFile(@"script\field\m13.scripb", this),
                new DCPBFile(@"script\field\m14.scripb", this),
                new DCPBFile(@"script\field\m15.scripb", this),
                new DCPBFile(@"script\field\m16.scripb", this),
                new DCPBFile(@"script\field\m17.scripb", this),
                new DCPBFile(@"script\field\m18.scripb", this),
                new DCPBFile(@"script\field\m19.scripb", this),
                new DCPBFile(@"script\field\m20.scripb", this),
                new DCPBFile(@"script\field\m22.scripb", this),
                new DCPBFile(@"script\field\m23.scripb", this),
                new DCPBFile(@"script\field\m24.scripb", this),
                new DCPBFile(@"script\field\m25.scripb", this),
                new DCPBFile(@"script\field\m26.scripb", this),
                new DCPBFile(@"script\field\m27.scripb", this),
                new DCPBFile(@"script\field\m28.scripb", this),
                new DCPBFile(@"script\field\m29.scripb", this),
                new DCPBFile(@"script\field\m30.scripb", this),
                new DCPBFile(@"script\field\m31.scripb", this),
                new DCPBFile(@"script\field\m32.scripb", this),
                new DCPBFile(@"script\field\m33.scripb", this),
                new DCPBFile(@"script\field\m34.scripb", this),
                new DCPBFile(@"script\field\m37.scripb", this),
                new DCPBFile(@"script\field\m38.scripb", this),
                new DCPBFile(@"script\field\m39.scripb", this),
                new DCPBFile(@"script\field\m40.scripb", this),
                new DCPBFile(@"script\field\m41.scripb", this),
                new DCPBFile(@"script\field\m42.scripb", this),
                new DCPBFile(@"script\field\me1.scripb", this),
                new DCPBFile(@"script\field\me2.scripb", this),
                new DCPBFile(@"script\field\pco.scripb", this),
                new DCPBFile(@"script\field\pillar01.scripb", this),
                new DCPBFile(@"script\field\po1.scripb", this),
                new DCPBFile(@"script\field\po2.scripb", this),
                new DCPBFile(@"script\field\po3.scripb", this),
                new DCPBFile(@"script\field\reio00.scripb", this),
                new DCPBFile(@"script\field\ro1.scripb", this),
                new DCPBFile(@"script\field\ro2.scripb", this),
                new DCPBFile(@"script\field\rp1.scripb", this),
                new DCPBFile(@"script\field\rp2.scripb", this),
                new DCPBFile(@"script\field\s01.scripb", this),
                new DCPBFile(@"script\field\s02.scripb", this),
                new DCPBFile(@"script\field\s03.scripb", this),
                new DCPBFile(@"script\field\s04.scripb", this),
                new DCPBFile(@"script\field\s05.scripb", this),
                new DCPBFile(@"script\field\s06.scripb", this),
                new DCPBFile(@"script\field\ta1.scripb", this),
                new DCPBFile(@"script\field\ur0.scripb", this),
                new DCPBFile(@"script\field\ur1.scripb", this),
                new DCPBFile(@"script\field\ura.scripb", this),
                new DCPBFile(@"script\field\w01d01a02.scripb", this),
                new DCPBFile(@"script\field\w01d01a03.scripb", this),
                new DCPBFile(@"script\field\w01d02a01.scripb", this),
                new DCPBFile(@"script\field\w01d02a02.scripb", this),
                new DCPBFile(@"script\field\w01d02a03.scripb", this),
                new DCPBFile(@"script\field\w01d02a04.scripb", this),
                new DCPBFile(@"script\field\w01d02a05.scripb", this),
                new DCPBFile(@"script\field\w01d02a06.scripb", this),
                new DCPBFile(@"script\field\w01d03a01.scripb", this),
                new DCPBFile(@"script\field\w01d03a02.scripb", this),
                new DCPBFile(@"script\field\w01d03a03.scripb", this),
                new DCPBFile(@"script\field\w01d03a04.scripb", this),
                new DCPBFile(@"script\field\w01d03a05.scripb", this),
                new DCPBFile(@"script\field\w01d03a06.scripb", this),
                new DCPBFile(@"script\field\w01f02a01.scripb", this),
                new DCPBFile(@"script\field\w01f03a01.scripb", this),
                new DCPBFile(@"script\field\w01f04a01.scripb", this),
                new DCPBFile(@"script\field\w01f05a01.scripb", this),
                new DCPBFile(@"script\field\w01t01a01.scripb", this),
                new DCPBFile(@"script\field\w01t01a02.scripb", this),
                new DCPBFile(@"script\field\w01t01a05.scripb", this),
                new DCPBFile(@"script\field\w01t01a09.scripb", this),
                new DCPBFile(@"script\field\w01t02a01.scripb", this),
                new DCPBFile(@"script\field\w01t02a05.scripb", this),
                new DCPBFile(@"script\field\w01t04a01.scripb", this),
                new DCPBFile(@"script\field\w01t04a02.scripb", this),
                new DCPBFile(@"script\field\w01t05a01.scripb", this),
                new DCPBFile(@"script\field\w02d01a01.scripb", this),
                new DCPBFile(@"script\field\w02d01a02.scripb", this),
                new DCPBFile(@"script\field\w02d01a03.scripb", this),
                new DCPBFile(@"script\field\w02d01a04.scripb", this),
                new DCPBFile(@"script\field\w02d01a05.scripb", this),
                new DCPBFile(@"script\field\w02d01a06.scripb", this),
                new DCPBFile(@"script\field\w02d01a07.scripb", this),
                new DCPBFile(@"script\field\w02d01a10.scripb", this),
                new DCPBFile(@"script\field\w02d01a21.scripb", this),
                new DCPBFile(@"script\field\w02d01a23.scripb", this),
                new DCPBFile(@"script\field\w02d01a24.scripb", this),
                new DCPBFile(@"script\field\w02d01a31.scripb", this),
                new DCPBFile(@"script\field\w02d01a32.scripb", this),
                new DCPBFile(@"script\field\w02f01a01.scripb", this),
                new DCPBFile(@"script\field\w02f02a01.scripb", this),
                new DCPBFile(@"script\field\w02t01a01.scripb", this),
                new DCPBFile(@"script\field\w02t01a05.scripb", this),
                new DCPBFile(@"script\field\w02t02a01.scripb", this),
                new DCPBFile(@"script\field\w02t03a01.scripb", this),
                new DCPBFile(@"script\field\w03d01a01.scripb", this),
                new DCPBFile(@"script\field\w03d01a03.scripb", this),
                new DCPBFile(@"script\field\w03d02a01.scripb", this),
                new DCPBFile(@"script\field\w03d02a02.scripb", this),
                new DCPBFile(@"script\field\w03d02a03.scripb", this),
                new DCPBFile(@"script\field\w03d02a04.scripb", this),
                new DCPBFile(@"script\field\w03d02a05.scripb", this),
                new DCPBFile(@"script\field\w03d02a06.scripb", this),
                new DCPBFile(@"script\field\w03d02a07.scripb", this),
                new DCPBFile(@"script\field\w03d02a08.scripb", this),
                new DCPBFile(@"script\field\w03f01a01.scripb", this),
                new DCPBFile(@"script\field\w03f02a01.scripb", this),
                new DCPBFile(@"script\field\w03t01a01.scripb", this),
                new DCPBFile(@"script\field\w03t01a02.scripb", this),
                new DCPBFile(@"script\field\w03t01a03.scripb", this),
                new DCPBFile(@"script\field\w03t01a04.scripb", this),
                new DCPBFile(@"script\field\w03t01a05.scripb", this),
                new DCPBFile(@"script\field\w03t01a06.scripb", this),
                new DCPBFile(@"script\field\w03t01a07.scripb", this),
                new DCPBFile(@"script\field\w03t02a01.scripb", this),
                new DCPBFile(@"script\field\w04d01a05.scripb", this),
                new DCPBFile(@"script\field\w04d01a09.scripb", this),
                new DCPBFile(@"script\field\w04d01a10.scripb", this),
                new DCPBFile(@"script\field\w04d01a11.scripb", this),
                new DCPBFile(@"script\field\w04d02a04.scripb", this),
                new DCPBFile(@"script\field\w04d03a01.scripb", this),
                new DCPBFile(@"script\field\w04f01a01.scripb", this),
                new DCPBFile(@"script\field\w04f03a01.scripb", this),
                new DCPBFile(@"script\field\w04f05a01.scripb", this),
                new DCPBFile(@"script\field\w04t01a02.scripb", this),
                new DCPBFile(@"script\field\w04t02a01.scripb", this),
                new DCPBFile(@"script\field\w04t02a05.scripb", this),
                new DCPBFile(@"script\field\w04t02a06.scripb", this),
                new DCPBFile(@"script\field\w04t03a01.scripb", this),
                new DCPBFile(@"script\field\w04t03a02.scripb", this),
                new DCPBFile(@"script\field\w04t03a05.scripb", this),
                new DCPBFile(@"script\field\w04t04a01.scripb", this),
                new DCPBFile(@"script\field\w05d01a01.scripb", this),
                new DCPBFile(@"script\field\w05d01a02.scripb", this),
                new DCPBFile(@"script\field\w05d01a03.scripb", this),
                new DCPBFile(@"script\field\w05d01a04.scripb", this),
                new DCPBFile(@"script\field\w05d01a05.scripb", this),
                new DCPBFile(@"script\field\w05d01a06.scripb", this),
                new DCPBFile(@"script\field\w05d01a07.scripb", this),
                new DCPBFile(@"script\field\w05d01a08.scripb", this),
                new DCPBFile(@"script\field\w05d01a09.scripb", this),
                new DCPBFile(@"script\field\w05d01a10.scripb", this),
                new DCPBFile(@"script\field\w05d01a11.scripb", this),
                new DCPBFile(@"script\field\w05f01a01.scripb", this),
                new DCPBFile(@"script\field\w05t01a01.scripb", this),
                new DCPBFile(@"script\field\w05t02a01.scripb", this),
                new DCPBFile(@"script\field\w06d01a02.scripb", this),
                new DCPBFile(@"script\field\w06d01a03.scripb", this),
                new DCPBFile(@"script\field\w06d01a04.scripb", this),
                new DCPBFile(@"script\field\w06d01a06.scripb", this),
                new DCPBFile(@"script\field\w06d02a01.scripb", this),
                new DCPBFile(@"script\field\w06d02a02.scripb", this),
                new DCPBFile(@"script\field\w06d02a03.scripb", this),
                new DCPBFile(@"script\field\w06d02a04.scripb", this),
                new DCPBFile(@"script\field\w06d02a05.scripb", this),
                new DCPBFile(@"script\field\w06d02a06.scripb", this),
                new DCPBFile(@"script\field\w06d02a07.scripb", this),
                new DCPBFile(@"script\field\w06f02a01.scripb", this),
                new DCPBFile(@"script\field\w06f03a01.scripb", this),
                new DCPBFile(@"script\field\w06t01a01.scripb", this),
                new DCPBFile(@"script\field\w06t01a03.scripb", this),
                new DCPBFile(@"script\field\w06t01a05.scripb", this),
                new DCPBFile(@"script\field\w06t01a06.scripb", this),
                new DCPBFile(@"script\field\w06t01a07.scripb", this),
                new DCPBFile(@"script\field\w06t01a08.scripb", this),
                new DCPBFile(@"script\field\w06t01a09.scripb", this),
                new DCPBFile(@"script\field\w06t02a01.scripb", this),
                new DCPBFile(@"script\field\w06t03a04.scripb", this),
                new DCPBFile(@"script\field\w06t03a05.scripb", this),
                new DCPBFile(@"script\field\w06t04a01.scripb", this),
                new DCPBFile(@"script\field\w07d01a01.scripb", this),
                new DCPBFile(@"script\field\w07d01a04.scripb", this),
                new DCPBFile(@"script\field\w07d01a05.scripb", this),
                new DCPBFile(@"script\field\w07d02a01.scripb", this),
                new DCPBFile(@"script\field\w07d02a02.scripb", this),
                new DCPBFile(@"script\field\w07d02a03.scripb", this),
                new DCPBFile(@"script\field\w07d02a06.scripb", this),
                new DCPBFile(@"script\field\w07d02a07.scripb", this),
                new DCPBFile(@"script\field\w07d02a09.scripb", this),
                new DCPBFile(@"script\field\w07d02a10.scripb", this),
                new DCPBFile(@"script\field\w07f01a01.scripb", this),
                new DCPBFile(@"script\field\w07f02a01.scripb", this),
                new DCPBFile(@"script\field\w07t01a01.scripb", this),
                new DCPBFile(@"script\field\w07t01a02.scripb", this),
                new DCPBFile(@"script\field\w07t01a03.scripb", this),
                new DCPBFile(@"script\field\w07t01a04.scripb", this),
                new DCPBFile(@"script\field\w07t01a06.scripb", this),
                new DCPBFile(@"script\field\w07t01a08.scripb", this),
                new DCPBFile(@"script\field\w07t01a10.scripb", this),
                new DCPBFile(@"script\field\w07t02a01.scripb", this),
                new DCPBFile(@"script\field\w07t02a03.scripb", this),
                new DCPBFile(@"script\field\w07t02a06.scripb", this),
                new DCPBFile(@"script\field\w07t02a07.scripb", this),
                new DCPBFile(@"script\field\w07t02a08.scripb", this),
                new DCPBFile(@"script\field\w07t03a01.scripb", this),
                new DCPBFile(@"script\field\w07t04a01.scripb", this),
                new DCPBFile(@"script\field\w08d01a06.scripb", this),
                new DCPBFile(@"script\field\w08d01a14.scripb", this),
                new DCPBFile(@"script\field\w08d01a15.scripb", this),
                new DCPBFile(@"script\field\w08d01a16.scripb", this),
                new DCPBFile(@"script\field\w08f01a01.scripb", this),
                new DCPBFile(@"script\field\w08f02a01.scripb", this),
                new DCPBFile(@"script\field\w08t01a01.scripb", this),
                new DCPBFile(@"script\field\w08t01a02.scripb", this),
                new DCPBFile(@"script\field\w09d01a01.scripb", this),
                new DCPBFile(@"script\field\w09d01a03.scripb", this),
                new DCPBFile(@"script\field\w09d01a07.scripb", this),
                new DCPBFile(@"script\field\w09d01a09.scripb", this),
                new DCPBFile(@"script\field\w09d01a13.scripb", this),
                new DCPBFile(@"script\field\w09d01a16.scripb", this),
                new DCPBFile(@"script\field\w09f01a01.scripb", this),
                new DCPBFile(@"script\field\w09t01a01.scripb", this),
                new DCPBFile(@"script\field\w09t01a05.scripb", this),
                new DCPBFile(@"script\field\w09t02a01.scripb", this),
                new DCPBFile(@"script\field\w09t02a02.scripb", this),
                new DCPBFile(@"script\field\w10d01a05.scripb", this),
                new DCPBFile(@"script\field\w10d01a07.scripb", this),
                new DCPBFile(@"script\field\w10d01a08.scripb", this),
                new DCPBFile(@"script\field\w10d02a01.scripb", this),
                new DCPBFile(@"script\field\w10d02a02.scripb", this),
                new DCPBFile(@"script\field\w10d02a03.scripb", this),
                new DCPBFile(@"script\field\w10t01a01.scripb", this),
                new DCPBFile(@"script\field\w10t01a02.scripb", this),
                new DCPBFile(@"script\field\w10t01a03.scripb", this),
                new DCPBFile(@"script\field\w10t01a04.scripb", this),
                new DCPBFile(@"script\field\w10t01a05.scripb", this),
                new DCPBFile(@"script\field\w10t01a06.scripb", this),
                new DCPBFile(@"script\field\w10t01a07.scripb", this),
                new DCPBFile(@"script\field\w10t01a08.scripb", this),
                new DCPBFile(@"script\field\w10t01a09.scripb", this),
                new DCPBFile(@"script\field\w10t01a10.scripb", this),
                new DCPBFile(@"script\field\w10t01a11.scripb", this),
                new DCPBFile(@"script\field\w10t01a12.scripb", this),
                new DCPBFile(@"script\field\w10t01a13.scripb", this),
                new DCPBFile(@"script\field\w10t01a14.scripb", this),
                new DCPBFile(@"script\field\w10t02a01.scripb", this),
                new DCPBFile(@"script\field\w10t02a02.scripb", this),
                new DCPBFile(@"script\field\w11f01a01.scripb", this),
                new DCPBFile(@"script\field\w11t02a01.scripb", this),
                new DCPBFile(@"script\field\w12d01a01.scripb", this),
                new DCPBFile(@"script\field\w12d01a02.scripb", this),
                new DCPBFile(@"script\field\w12d01a03.scripb", this),
                new DCPBFile(@"script\field\w12d01a04.scripb", this),
                new DCPBFile(@"script\field\w12d01a05.scripb", this),
                new DCPBFile(@"script\field\w12d01a06.scripb", this),
                new DCPBFile(@"script\field\w12d01a07.scripb", this),
                new DCPBFile(@"script\field\w12d01a08.scripb", this),
                new DCPBFile(@"script\field\w12t01a05.scripb", this),
                new DCPBFile(@"script\field\w12t01a06.scripb", this),
                new DCPBFile(@"script\field\w12t01a07.scripb", this),
                new DCPBFile(@"script\field\w12t01a08.scripb", this),
                new DCPBFile(@"script\field\w12t02a01.scripb", this),
                new DCPBFile(@"script\field\w13d01a14.scripb", this),
                new DCPBFile(@"script\field\w13d01a15.scripb", this),
                new DCPBFile(@"script\field\w13d01a25.scripb", this),
                new DCPBFile(@"script\field\w13d01a30.scripb", this),
                new DCPBFile(@"script\field\w13d01a31.scripb", this),
                new DCPBFile(@"script\field\w13f01a01.scripb", this),
                new DCPBFile(@"script\field\w13t01a01.scripb", this),
                new DCPBFile(@"script\field\w14t01a06.scripb", this),
                new DCPBFile(@"script\field\w14t01a07.scripb", this),
                new DCPBFile(@"script\field\w14t01a08.scripb", this),
                new DCPBFile(@"script\field\w15f01a01.scripb", this),
                new DCPBFile(@"script\field\w15f02a01.scripb", this),
                new DCPBFile(@"script\field\w17f01a01.scripb", this),
                new DCPBFile(@"script\field\w17f05a01.scripb", this),
                new DCPBFile(@"script\field\w18f01a01.scripb", this),
                new DCPBFile(@"script\field\w18f02a01.scripb", this),
                new DCPBFile(@"script\field\w18f02a02.scripb", this),
                new DCPBFile(@"script\player\rm_trap.scripb", this),
                new DCPBFile(@"script\scene\chain_free.scripb", this),
                new DCPBFile(@"script\scene\es_fail.scripb", this),
                new DCPBFile(@"script\scene\limit_drive.scripb", this),
                new DCPBFile(@"script\scene\lsc_atk.scripb", this),
                new DCPBFile(@"script\scene\lsc_bom.scripb", this),
                new DCPBFile(@"script\scene\lsc_catk.scripb", this),
                new DCPBFile(@"script\scene\lsc_mag.scripb", this),
                new DCPBFile(@"script\scene\lsc_sht.scripb", this),
                new DCPBFile(@"script\scene\lsc_thw.scripb", this),
                new DCPBFile(@"script\scene\s285.scripb", this),
                new DCPBFile(@"script\scene\s286.scripb", this),
                new DCPBFile(@"script\scene\s292.scripb", this),
                new DCPBFile(@"script\scene\s297.scripb", this),
                new DCPBFile(@"script\scene\s298.scripb", this),
                new DCPBFile(@"script\scene\s299.scripb", this),
                new DCPBFile(@"script\scene\s303.scripb", this),
                new DCPBFile(@"script\scene\s304.scripb", this),
                new DCPBFile(@"script\scene\scn_aatk.scripb", this),
                new DCPBFile(@"script\scene\scn_bom.scripb", this),
                new DCPBFile(@"script\scene\scn_con.scripb", this),
                new DCPBFile(@"script\scene\scn_evt.scripb", this),
                new DCPBFile(@"script\scene\scn_ext.scripb", this),
                new DCPBFile(@"script\scene\scn_gatk.scripb", this),
                new DCPBFile(@"script\scene\scn_latk.scripb", this),
                new DCPBFile(@"script\scene\scn_lds.scripb", this),
                new DCPBFile(@"script\scene\scn_leo.scripb", this),
                new DCPBFile(@"script\scene\scn_meuse.scripb", this),
                new DCPBFile(@"script\scene\scn_muse.scripb", this),
                new DCPBFile(@"script\scene\scn_rate.scripb", this),
                new DCPBFile(@"script\scene\scn_satk.scripb", this),
                new DCPBFile(@"script\scene\scn_spp.scripb", this),
                new DCPBFile(@"script\scene\scn_thw.scripb", this),
                new DCPBFile(@"script\scene\srec.scripb", this),
                new DCPBFile(@"script\scene\tanren.scripb", this),
                new DCPBFile(@"script\symbol\275_0.scripb", this),
                new DCPBFile(@"script\symbol\apollon.scripb", this),
                new DCPBFile(@"script\symbol\arei.scripb", this),
                new DCPBFile(@"script\symbol\arms100.scripb", this),
                new DCPBFile(@"script\symbol\arms101.scripb", this),
                new DCPBFile(@"script\symbol\arms200.scripb", this),
                new DCPBFile(@"script\symbol\arms201.scripb", this),
                new DCPBFile(@"script\symbol\arms300.scripb", this),
                new DCPBFile(@"script\symbol\arms400.scripb", this),
                new DCPBFile(@"script\symbol\arms401.scripb", this),
                new DCPBFile(@"script\symbol\arms500.scripb", this),
                new DCPBFile(@"script\symbol\arms600.scripb", this),
                new DCPBFile(@"script\symbol\arms601.scripb", this),
                new DCPBFile(@"script\symbol\arms700.scripb", this),
                new DCPBFile(@"script\symbol\arms701.scripb", this),
                new DCPBFile(@"script\symbol\arms900.scripb", this),
                new DCPBFile(@"script\symbol\arms901.scripb", this),
                new DCPBFile(@"script\symbol\armsS01.scripb", this),
                new DCPBFile(@"script\symbol\armsS02.scripb", this),
                new DCPBFile(@"script\symbol\atn00.scripb", this),
                new DCPBFile(@"script\symbol\bainin.scripb", this),
                new DCPBFile(@"script\symbol\eute.scripb", this),
                new DCPBFile(@"script\symbol\giyugun.scripb", this),
                new DCPBFile(@"script\symbol\go_mo1.scripb", this),
                new DCPBFile(@"script\symbol\go_moira.scripb", this),
                new DCPBFile(@"script\symbol\go_muse.scripb", this),
                new DCPBFile(@"script\symbol\go_m_00.scripb", this),
                new DCPBFile(@"script\symbol\inn100.scripb", this),
                new DCPBFile(@"script\symbol\item100.scripb", this),
                new DCPBFile(@"script\symbol\item101.scripb", this),
                new DCPBFile(@"script\symbol\item200.scripb", this),
                new DCPBFile(@"script\symbol\item201.scripb", this),
                new DCPBFile(@"script\symbol\item300.scripb", this),
                new DCPBFile(@"script\symbol\item301.scripb", this),
                new DCPBFile(@"script\symbol\item400.scripb", this),
                new DCPBFile(@"script\symbol\item401.scripb", this),
                new DCPBFile(@"script\symbol\item500.scripb", this),
                new DCPBFile(@"script\symbol\item600.scripb", this),
                new DCPBFile(@"script\symbol\item601.scripb", this),
                new DCPBFile(@"script\symbol\item700.scripb", this),
                new DCPBFile(@"script\symbol\item701.scripb", this),
                new DCPBFile(@"script\symbol\item900.scripb", this),
                new DCPBFile(@"script\symbol\item901.scripb", this),
                new DCPBFile(@"script\symbol\itemS01.scripb", this),
                new DCPBFile(@"script\symbol\itemS02.scripb", this),
                new DCPBFile(@"script\symbol\jamp0.scripb", this),
                new DCPBFile(@"script\symbol\jamp1.scripb", this),
                new DCPBFile(@"script\symbol\jamp2.scripb", this),
                new DCPBFile(@"script\symbol\jamp3.scripb", this),
                new DCPBFile(@"script\symbol\jamp4.scripb", this),
                new DCPBFile(@"script\symbol\k08.scripb", this),
                new DCPBFile(@"script\symbol\kai.scripb", this),
                new DCPBFile(@"script\symbol\kari.scripb", this),
                new DCPBFile(@"script\symbol\kari00.scripb", this),
                new DCPBFile(@"script\symbol\m10_01.scripb", this),
                new DCPBFile(@"script\symbol\m35.scripb", this),
                new DCPBFile(@"script\symbol\m36.scripb", this),
                new DCPBFile(@"script\symbol\meru.scripb", this),
                new DCPBFile(@"script\symbol\moirai.scripb", this),
                new DCPBFile(@"script\symbol\muse01.scripb", this),
                new DCPBFile(@"script\symbol\muse02.scripb", this),
                new DCPBFile(@"script\symbol\muse03.scripb", this),
                new DCPBFile(@"script\symbol\muse05.scripb", this),
                new DCPBFile(@"script\symbol\muse08.scripb", this),
                new DCPBFile(@"script\symbol\muse_twin.scripb", this),
                new DCPBFile(@"script\symbol\muse_w.scripb", this),
                new DCPBFile(@"script\symbol\new_npc.scripb", this),
                new DCPBFile(@"script\symbol\ogosyo.scripb", this),
                new DCPBFile(@"script\symbol\pori.scripb", this),
                new DCPBFile(@"script\symbol\pub101.scripb", this),
                new DCPBFile(@"script\symbol\pub200.scripb", this),
                new DCPBFile(@"script\symbol\pub300.scripb", this),
                new DCPBFile(@"script\symbol\pub401.scripb", this),
                new DCPBFile(@"script\symbol\pub402.scripb", this),
                new DCPBFile(@"script\symbol\pub500.scripb", this),
                new DCPBFile(@"script\symbol\pub600.scripb", this),
                new DCPBFile(@"script\symbol\pub900.scripb", this),
                new DCPBFile(@"script\symbol\pubS02.scripb", this),
                new DCPBFile(@"script\symbol\reio.scripb", this),
                new DCPBFile(@"script\symbol\reio00.scripb", this),
                new DCPBFile(@"script\symbol\repu.scripb", this),
                new DCPBFile(@"script\symbol\souko.scripb", this),
                new DCPBFile(@"script\symbol\t100_01.scripb", this),
                new DCPBFile(@"script\symbol\t501_01.scripb", this),
                new DCPBFile(@"script\symbol\t600_01.scripb", this),
                new DCPBFile(@"script\symbol\t600_02.scripb", this),
                new DCPBFile(@"script\symbol\tare.scripb", this),
                new DCPBFile(@"script\symbol\tenti01.scripb", this),
                new DCPBFile(@"script\symbol\tenti02.scripb", this),
                new DCPBFile(@"script\symbol\tenti03.scripb", this),
                new DCPBFile(@"script\symbol\tougi.scripb", this),
                new DCPBFile(@"script\symbol\tougi_01.scripb", this),
                new DCPBFile(@"script\symbol\tougi_02.scripb", this),
                new DCPBFile(@"script\symbol\town000.scripb", this),
                new DCPBFile(@"script\symbol\town001.scripb", this),
                new DCPBFile(@"script\symbol\town002.scripb", this),
                new DCPBFile(@"script\symbol\town003.scripb", this),
                new DCPBFile(@"script\symbol\town100.scripb", this),
                new DCPBFile(@"script\symbol\town101.scripb", this),
                new DCPBFile(@"script\symbol\town102.scripb", this),
                new DCPBFile(@"script\symbol\town103.scripb", this),
                new DCPBFile(@"script\symbol\town104.scripb", this),
                new DCPBFile(@"script\symbol\town200.scripb", this),
                new DCPBFile(@"script\symbol\town201.scripb", this),
                new DCPBFile(@"script\symbol\town300.scripb", this),
                new DCPBFile(@"script\symbol\town400.scripb", this),
                new DCPBFile(@"script\symbol\town401.scripb", this),
                new DCPBFile(@"script\symbol\town402.scripb", this),
                new DCPBFile(@"script\symbol\town500.scripb", this),
                new DCPBFile(@"script\symbol\town501.scripb", this),
                new DCPBFile(@"script\symbol\town600.scripb", this),
                new DCPBFile(@"script\symbol\town601.scripb", this),
                new DCPBFile(@"script\symbol\town700.scripb", this),
                new DCPBFile(@"script\symbol\town701.scripb", this),
                new DCPBFile(@"script\symbol\town702.scripb", this),
                new DCPBFile(@"script\symbol\town703.scripb", this),
                new DCPBFile(@"script\symbol\town704.scripb", this),
                new DCPBFile(@"script\symbol\town705.scripb", this),
                new DCPBFile(@"script\symbol\town900.scripb", this),
                new DCPBFile(@"script\symbol\townS02.scripb", this),
                new DCPBFile(@"script\symbol\tS02_01.scripb", this),
                new DCPBFile(@"script\symbol\urania.scripb", this),
                new DCPBFile(@"script\symbol\vns.scripb", this),
                new DCPBFile(@"script\symbol\w01.scripb", this),
                new DCPBFile(@"script\symbol\w02.scripb", this),
                new DCPBFile(@"script\symbol\w03.scripb", this),
                new DCPBFile(@"script\symbol\w04.scripb", this),
                new DCPBFile(@"script\symbol\w05.scripb", this),
                new DCPBFile(@"script\symbol\w12_01.scripb", this),
                new DCPBFile(@"script\symbol\w12_02.scripb", this),
                new DCPBFile(@"script\symbol\w12_03.scripb", this),
                new DCPBFile(@"script\symbol\world13.scripb", this),
                new DCPBFile(@"script\treasure\appear.scripb", this),
                new DCPBFile(@"script\treasure\open.scripb", this),
                new DCPBFile(@"script\treasure\trap.scripb", this),
                new MusicBoxFile(@"sound\sound_musicbox.bin", this)
            };
            
            foreach (ITextFile file in textFiles)
            {
                this.Files.TextFiles.Add(file);
            }
        }
        
        /// <summary>
        /// Converts a range of bytes into a text string using the project's Encoding.
        /// </summary>
        /// <param name="data">A byte array containing the data to be converted.</param>
        /// <returns>The converted text string.</returns>
        public override string GetText(byte[] data)
        {
            return this.GetText(data, 0);
        }
        
        /// <summary>
        /// Converts a range of bytes into a text string using the project's Encoding.
        /// </summary>
        /// <param name="data">A byte array containing the data to be converted.</param>
        /// <param name="offset">The starting offset of the data in the array.</param>
        /// <returns>The converted text string.</returns>
        public override string GetText(byte[] data, int offset)
        {
            return this.GetText(data, offset, data.Length).Replace("\r", @"\r").Replace("\n", @"\n");
        }
        
        /// <summary>
        /// Converts a range of bytes into a text string using the project's Encoding.
        /// </summary>
        /// <param name="data">A byte array containing the data to be converted.</param>
        /// <param name="offset">The starting offset of the data in the array.</param>
        /// <param name="length">The maximum number of bytes to convert.</param>
        /// <returns>The converted text string.</returns>
        public override string GetText(byte[] data, int offset, int length)
        {
            string text = string.Empty;
            for (int i = offset; i < offset + length && i < data.Length; i++)
            {
                // End of string
                if (data[i] == 0x00)
                {
                    break;
                }
                else if (data[i] < 0x80 || (data[i] >= 0xA0 && data[i] <= 0xDF))
                {
                    text += Encoding.GetString(data, i, 1);
                }
                else
                {
                    string character = Encoding.GetString(data, i, 2);
                    text += character;
                    i++;
                }
            }
            
            return text;
        }
        
        /// <summary>
        /// Converts a text string into an array of bytes using the project's Encoding.
        /// </summary>
        /// <param name="text">The text string to be converted.</param>
        /// <returns>The converted byte array.</returns>
        public override byte[] GetBytes(string text)
        {
            List<byte> bytes = new List<byte>();
            while (text.Length > 0)
            {
                byte[] charData = Encoding.GetBytes(text.Substring(0, 1));
                for (int i = 0; i < charData.Length; i++)
                {
                    bytes.Add(charData[i]);
                }
                
                text = text.Substring(1);
            }
            
            return bytes.ToArray();
        }
    }
}
