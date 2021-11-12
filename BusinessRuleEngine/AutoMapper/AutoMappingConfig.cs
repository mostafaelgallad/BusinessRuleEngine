using AutoMapper;
using BusinessRuleEngine.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessRuleEngine.AutoMapper
{
    public class AutoMappingConfig : Profile
    {
        public AutoMappingConfig()
        {
            //CreateMap<DbDataAdapter, ContriesFromTestDB>().ForMember(a=>a.Actions , c=>c.MapFrom(p=>p.Fill(ContriesFromTestDB)));
            //MapperRegistry.Mappers.Add
        }
    }
}
