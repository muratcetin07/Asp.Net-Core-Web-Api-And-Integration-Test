﻿using Model;
using Model.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Abstract
{
    public interface IProductDataRepo : IData<Product>
    {
        decimal GetTotalProductPrice();
    }
}
