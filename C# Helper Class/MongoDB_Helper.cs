using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Core;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// @author Sourav Kumar
/// </summary>
public class Mongodb_Helper
{
    public bool _rval;

    public Mongodb_Helper()
    {
        //TODO: Add constructor logic here

        //References

        //www.codeproject.com/Articles/87757/MongoDB-and-C
        //docs.mongodb.com/getting-started/csharp/remove/
        //api.mongodb.com/csharp/current/html/T_MongoDB_Driver_FilterDefinitionBuilder_1.htm
    }


    public static MongoClient _client = new MongoClient("mongodb://localhost:27017");
    public static IMongoDatabase _database = _client.GetDatabase("DataBase Name");

    //select 

    #region Selection 

    public List<BsonDocument> Select_Text_Comp_Select(string Table_Name, string col_name, string para)
    {

        Table_Name = Table_Name.Trim();
        col_name = col_name.Trim();
        para = para.Trim();

        var collection = _database.GetCollection<BsonDocument>(Table_Name.Trim());

        var filter = new BsonDocument { { col_name, new BsonDocument { { "$regex", "^" + para.Trim() }, { "$options", "i" } } } };

        if (!string.IsNullOrEmpty(para.Trim()))
        {
            var items_coll = collection.Find(filter).Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList();
            return items_coll;
        }
        else
        {
            var items_coll = collection.Find(new BsonDocument()).Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList();
            return items_coll;
        }
    }

    public List<BsonDocument> Select_Numers_Collection_Values(string Table_Name, string col_name, string para, string Compare_para)
    {
        Table_Name = Table_Name.Trim();
        col_name = col_name.Trim();
        para = para.Trim();
        Compare_para = Compare_para.Trim();


        var collection = _database.GetCollection<BsonDocument>(Table_Name.Trim());
        if (!string.IsNullOrEmpty(para.Trim()))
        {
            if (Compare_para.ToLower() == "l")
            {
                var filter = Builders<BsonDocument>.Filter.Lt(new StringFieldDefinition<BsonDocument, BsonDecimal128>(col_name.Trim()), new BsonDecimal128(Convert.ToInt64(para.Trim())));
                var items_coll = collection.Find(filter).Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList();
                return items_coll;

            }
            else if (Compare_para.ToLower() == "g")
            {
                var filter = Builders<BsonDocument>.Filter.Gt(new StringFieldDefinition<BsonDocument, BsonDecimal128>(col_name.Trim()), new BsonDecimal128(Convert.ToInt64(para.Trim())));
                var items_coll = collection.Find(filter).Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList();
                return items_coll;

            }
            else if (Compare_para.ToLower() == "e")
            {
                var filter = Builders<BsonDocument>.Filter.Eq(new StringFieldDefinition<BsonDocument, BsonDecimal128>(col_name.Trim()), new BsonDecimal128(Convert.ToInt64(para.Trim())));
                var items_coll = collection.Find(filter).Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList();
                return items_coll;
            }
            else if (Compare_para.ToLower() == "le")
            {
                var filter = Builders<BsonDocument>.Filter.Lte(new StringFieldDefinition<BsonDocument, BsonDecimal128>(col_name.Trim()), new BsonDecimal128(Convert.ToInt64(para.Trim())));
                var items_coll = collection.Find(filter).Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList();
                return items_coll;

            }
            else if (Compare_para.ToLower() == "ge")
            {
                var filter = Builders<BsonDocument>.Filter.Gte(new StringFieldDefinition<BsonDocument, BsonDecimal128>(col_name.Trim()), new BsonDecimal128(Convert.ToInt64(para.Trim())));
                var items_coll = collection.Find(filter).Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList();
                return items_coll;

            }
            else
            {
                var items_coll = collection.Find(new BsonDocument()).Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList();
                return items_coll;
            }

        }
        else
        {
            var items_coll = collection.Find(new BsonDocument()).Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList();
            return items_coll;
        }
    }

    public DataTable Select_Collection_Table(string Table_Name, string col_name, string para)
    {
        Table_Name = Table_Name.Trim();
        col_name = col_name.Trim();
        para = para.Trim();

        DataTable _dt = new DataTable();

        try
        {
            var collection = _database.GetCollection<BsonDocument>(Table_Name);
            var filter = new BsonDocument { { col_name, new BsonDocument { { "$regex", "^" + para }, { "$options", "i" } } } };


            List<BsonDocument> items_coll = new List<BsonDocument>();
            if (!string.IsNullOrEmpty(para.Trim()))
            {
                items_coll = collection.Find(filter).Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList();
            }
            else
            {
                items_coll = collection.Find(new BsonDocument()).Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList();
            }

            DataTable dt = new DataTable(); // Create empty datatable we will fill with data.

            foreach (BsonDocument obj in items_coll) // Loop thru all Bson documents returned from the query.
            {
                DataRow dr = dt.NewRow(); // Add new row to datatable.
                ExecuteFillDataTable(obj, dt, dr, string.Empty); // Recursuve method to loop thru al results json.
                dt.Rows.Add(dr); // Add the newly created datarow to the table
            }

            return dt;
        }
        catch (Exception ex)
        {



        }
        return _dt;
    }

    private void ExecuteFillDataTable(BsonDocument doc, DataTable dt, DataRow dr, string parent)
    {
        // arrays means 1:M relation to parent, meaning we will have to fake multi levels by adding 1 more row foreach item in array.
        // i created the here because i want to add all new array rows after our main row.
        List<KeyValuePair<string, BsonArray>> arrays = new List<KeyValuePair<string, BsonArray>>();

        foreach (string key in doc.Names) // this will loop thru all our json attributes.
        {
            object value = doc[key]; // get the value of the current json attribute.

            string x; // for my specific needs, i need all values to be save in datatable as strings. you can implument to match your needs.

            // if our attribute is BsonDocument, means relation is 1:1. we can add values to current datarow and call the data column "parent.current".
            // we will use this recursive method to run thru all the child document.
            if (value is BsonDocument)
            {
                string newParent = string.IsNullOrEmpty(parent) ? key : parent + "." + key;
                ExecuteFillDataTable((BsonDocument)value, dt, dr, newParent);
            }
            // if our attribute is BsonArray, means relation is 1:N. we will need to add new rows, but not now.
            // we will save it in queue for later use.
            else if (value is BsonArray)
            {
                // Save array to queue for later loop.
                arrays.Add(new KeyValuePair<string, BsonArray>(key, (BsonArray)value));


            }
            // if our attribute is datatime i needed it in a spesific string format.
            else if (value is BsonTimestamp)
            {
                x = doc[key].AsBsonTimestamp.ToLocalTime().ToString("s");

            }
            // if our attribute is null, i needed it converted to string.empty.
            else if (value is BsonNull)
            {
                x = string.Empty;

            }
            else
            {
                // for all other cases, just .ToString() it.
                x = value.ToString();

                // Make sure our datatable already contains column with the right name. if not - add it.
                string colName = string.IsNullOrEmpty(parent) ? key : parent + "." + key;
                if (!dt.Columns.Contains(colName))
                    dt.Columns.Add(colName);

                // Add the value to the datarow in the right column.
                dr[colName] = value;

            }

        }

        // loop thru all arrays when finish with standart fields.
        foreach (KeyValuePair<string, BsonArray> array in arrays)
        {
            // create column name that contains the parent name + child name.
            string newParent = string.IsNullOrEmpty(parent) ? array.Key : parent + "." + array.Key;

            // save the old - we will need it so we can add it existing values to the new row.
            DataRow drOld = dr;

            // loop thru all the BsonDocuments in the array
            foreach (BsonDocument doc2 in array.Value)
            {
                // Create new datarow for each item in array.
                dr = dt.NewRow();
                dr.ItemArray = drOld.ItemArray; // this will copy all the main row values to the new row - might not be needed for your use.
                dt.Rows.Add(dr); // the the new row to the datatable
                ExecuteFillDataTable(doc2, dt, dr, newParent); // fill the new datarow withh all the values for the BsonDocument in the array.
            }

            dr = drOld; // set the main data row back so we can use it values again.
        }
    }


    #endregion

    #region Insert 
    public bool Insert_data(string[,] val, string Table_Name)
    {
        Table_Name = Table_Name.Trim();

        _rval = false;
        try
        {
            if (val == null)
            {
                return false;
            }
            if (val.Length == 0)
            {
                return false;
            }

            BsonDocument bsonDoc = new BsonDocument();
            for (int i = 0; i < val.Length / 2; i++)
            {
                bsonDoc.Add(new BsonElement(val[i, 0].ToString(), val[i, 1].ToString()));
            }

            var collection = _database.GetCollection<BsonDocument>(Table_Name);
            collection.InsertOne(bsonDoc);
            return true;

        }
        catch (Exception ex)
        {
        }
        return _rval;
    }
    public bool Insert_data(List<BsonDocument> data, string Table_Name)
    {
        Table_Name = Table_Name.Trim();
        _rval = false;
        try
        {
            var collection = _database.GetCollection<BsonDocument>(Table_Name);
            collection.InsertMany(data);
            return true;
        }
        catch (Exception ex)
        {
        }
        return _rval;
    }
    public bool Insert_data(DataTable data, string Table_Name)
    {
        Table_Name = Table_Name.Trim();
        _rval = false;
        try
        {
            var collection = _database.GetCollection<BsonDocument>(Table_Name);
            List<BsonDocument> doc = new List<BsonDocument>();
            foreach (DataRow dr in data.Rows)
            {
                var dictionary = dr.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => dr[col.ColumnName]);
                doc.Add(new BsonDocument(dictionary));
            }

            collection.InsertMany(doc.AsEnumerable());

            return true;
        }
        catch (Exception ex)
        {
        }
        return _rval;
    }
    public bool Import_DataBase(DataTable _dt, string file_name)
    {
        file_name = file_name.Trim();
        _rval = false;

        try
        {
            if (_dt == null)
            {
                return false;
            }

            if (_dt.Rows.Count == 0)
            {
                return true;
            }
            var collection = _database.GetCollection<BsonDocument>(file_name);

            //  var filter = Builders<BsonDocument>.Filter.Gte(new StringFieldDefinition<BsonDocument, BsonInt32>("Id"), new BsonInt32(0));

            var filter = new BsonDocument();

            collection.DeleteMany(filter);

            List<BsonDocument> data = new List<BsonDocument>();
            foreach (DataRow dr in _dt.Rows)
            {
                var dictionary = dr.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => dr[col.ColumnName]);
                data.Add(new BsonDocument(dictionary));

            }

            collection.InsertMany(data.AsEnumerable());
            return true;
        }
        catch (Exception ex)
        {
            _rval = false;
        }
        return _rval;
    }
    #endregion

    #region  Update Operation

    public bool Update_Collection_Node(string File_Name, string Id, string col_name, string col_value)
    {

        File_Name = File_Name.Trim();
        Id = Id.Trim();
        col_name = col_name.Trim();
        col_value = col_value.Trim();

        _rval = false;
        try
        {
            var collection = _database.GetCollection<BsonDocument>(File_Name);
            var filter = Builders<BsonDocument>.Filter.Eq("Id", Id);
            var update = Builders<BsonDocument>.Update.Set(col_name, col_value);
            collection.UpdateOne(filter, update);
            return true;
        }
        catch (Exception ex)
        {


        }
        return _rval;
    }

    public bool Collection_Node_Handler(string master_id, double _rid, string database_Table_name, string col_name, string file_name)
    {

        master_id = master_id.Trim();
        database_Table_name = database_Table_name.Trim();
        col_name = col_name.Trim();
        file_name = file_name.Trim();

        _rval = false;
        DataTable _dt = new DataTable();
        SqlHelper _sql = new SqlHelper();
        try
        {
            if (_rid > 0)
            {
                Delete_Collection_Node(col_name.Trim(), _rid.ToString().Trim(), file_name.Trim());
                _dt = _sql.Get_DataTable(" select top 1 * from " + database_Table_name.Trim() + " where del_flag = 0  and Id = " + _rid + " ");
                Insert_data(_dt, file_name);
            }
            else if (!string.IsNullOrEmpty(master_id))
            {
                if (Convert.ToDouble(master_id) > 0)
                {
                    Delete_Collection_Node(col_name.Trim(), master_id.ToString().Trim(), file_name.Trim());
                    _dt = _sql.Get_DataTable(" select top 1 * from " + database_Table_name.Trim() + " where del_flag = 0  and Id = " + master_id.Trim() + " ");
                    Insert_data(_dt, file_name);

                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _sql = null;

        }
        finally
        {
            _sql = null;

        }
        return _rval;
    }

    #endregion

    #region Delete Operation 
    public bool Truncate_Collection(string file_name, string col_name, string id)
    {
        file_name = file_name.Trim();
        col_name = col_name.Trim();
        id = id.Trim();

        _rval = false;

        try
        {

            var collection = _database.GetCollection<BsonDocument>(file_name);

            var filter = Builders<BsonDocument>.Filter.Gte(new StringFieldDefinition<BsonDocument, BsonDecimal128>(col_name), new BsonDecimal128(Convert.ToInt64(id)));

            collection.DeleteMany(filter);

            return true;

        }
        catch (Exception ex)
        {
            return false;
        }

        return _rval;
    }

    public bool Delete_Collection_Node(string col_name, string id, string file_name)
    {
        file_name = file_name.Trim();
        col_name = col_name.Trim();
        id = id.Trim();

        _rval = false;

        try
        {
            var collection = _database.GetCollection<BsonDocument>(file_name);

            var filter = Builders<BsonDocument>.Filter.Eq(new StringFieldDefinition<BsonDocument, BsonDecimal128>(col_name), new BsonDecimal128(Convert.ToDecimal(id)));

            collection.DeleteMany(filter);

            return true;
        }
        catch (Exception ex)
        {

        }
        return _rval;
    }

    #endregion
}