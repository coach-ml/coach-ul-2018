using System;
using System.Collections.Generic;
using ReactUnity;

namespace Models
{
    [Serializable]
    public class ListModel : IModel
    {
        public List<ItemModel> Models;
    }
}