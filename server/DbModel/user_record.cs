//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DbModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class user_record
    {
        public int log_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<System.DateTime> login_time { get; set; }
        public Nullable<System.DateTime> logout_time { get; set; }
        public string login_ip { get; set; }
    }
}
