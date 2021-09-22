namespace RenamePhoto
{
    class DataMap
    {
        public string id_intelect;
        public string tabn_one_c;

        public DataMap(string id_intelect, string tabn_one_c)
        {
            this.id_intelect = id_intelect;
            this.tabn_one_c = create_try_tabn(tabn_one_c);
        }

        private string create_try_tabn(string tabn)
        {
            switch (tabn.Length)
            {
                case 1:
                    return "00000" + tabn;
                case 2:
                    return "0000" + tabn;
                case 3:
                    return "000" + tabn;
                case 4:
                    return "00" + tabn;
                case 5:
                    return "0" + tabn;
                default:
                    return tabn;
            }
        }
    }
}
