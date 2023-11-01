﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _0J01021_中村快_S3
{
    public class SQL
    {
        private string con = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\ckadai\\C#\\0J01021_中村快_S3\\0J01021_中村快_S3\\Database1.mdf;Integrated Security=True;Connect Timeout=30";

        private List<string> datas_name = new List<string>()
        {
            "@id","@do","@date","@category","@location","@url","@explanation","@alarm","@comp"
        };
        // パラメータの型　0：NVarChar、1：Int、2：DateTime
        private int[] para = new int[9]
        {
            1,0,2,0,0,0,0,2,1
        };

        public List<List<string>> Data_SelectAll(string s, int[] para)
        {
            List<List<string>> data_list = new List<List<string>>();

            using (var command = new SqlConnection(con)) 
            {
                command.Open();
                var cmd = command.CreateCommand();
                // SQL文の作成
                cmd.CommandText = s;

                var SQL = new SqlDataAdapter(cmd);
                SqlDataReader reader = cmd.ExecuteReader();

                // データが入っているか判定
                if (reader.HasRows)
                {
                    data_list.Clear();

                    // データを読み取り、2次元リスト配列に追加
                    while (reader.Read())
                    {
                        List<string> row = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            // 列のインデックスに応じてデータ型を指定
                            string data;
                            try
                            {
                                if (para[i] == 0)
                                {
                                    data = reader.GetString(i);
                                    row.Add(data);
                                }
                                else if (para[i] == 1)
                                {
                                    data = reader.GetInt32(i).ToString();
                                    row.Add(data);
                                }
                                else
                                {
                                    data = reader.GetDateTime(i).ToString("F");
                                    row.Add(data);
                                }
                            }
                            catch (SqlNullValueException) // nullの場合
                            {
                                row.Add("");
                            }
                        }

                        data_list.Add(row);
                    }
                }
                else // データが入っていない場合
                {
                    data_list.Clear();
                }

                command.Close();
            }

            return data_list;
        }

        public List<List<string>> Data_Select(string s, int[] para, string serch1,string serch2)
        {
            List<List<string>> data_list = new List<List<string>>();

            using (var command = new SqlConnection(con))
            {
                command.Open();
                var cmd = command.CreateCommand();
                // SQL文の作成
                cmd.CommandText = s;
                
                cmd.Parameters.Add(new SqlParameter("@serch1", serch1));
                cmd.Parameters.Add(new SqlParameter("@serch2", serch2));

                var SQL = new SqlDataAdapter(cmd);
                SqlDataReader reader = cmd.ExecuteReader();

                // データが入っているか判定
                if (reader.HasRows)
                {
                    data_list.Clear();

                    // データを読み取り、2次元リスト配列に追加
                    while (reader.Read())
                    {
                        List<string> row = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            // 列のインデックスに応じてデータ型を指定
                            string data;
                            try
                            {
                                if (para[i] == 0)
                                {
                                    data = reader.GetString(i);
                                    row.Add(data);
                                }
                                else if (para[i] == 1)
                                {
                                    data = reader.GetInt32(i).ToString();
                                    row.Add(data);
                                }
                                else
                                {
                                    data = reader.GetDateTime(i).ToString("F");
                                    row.Add(data);
                                }
                            }
                            catch (SqlNullValueException) // nullの場合
                            {
                                row.Add("");
                            }
                        }

                        data_list.Add(row);
                    }
                }
                else // データが入っていない場合
                {
                    data_list.Clear();
                }

                command.Close();
            }

            return data_list;
        }

        // データ挿入（正常に終了したらtrueを返す）
        public bool Data_Insert(List<string> datas)
        {
            using (var command = new SqlConnection(con))
            {
                command.Open();
                using (SqlTransaction transaction = command.BeginTransaction())
                {
                    try
                    {
                        string s = "INSERT INTO dbo.todo (Do,Date,Category,Location,Url,Explanation,Alarm,Comp) VALUES (@do,@date,@category,@location,@url,@explanation,@alarm,@comp)";
                        // データベースにデータを追加していく
                        using (SqlCommand cmd = new SqlCommand(s, command, transaction))
                        {
                            // パラメータ追加(何も設定されていなければNULLを入れる)
                            for (int i = 0; i < datas.Count; i++)
                            {
                                Parameters_Add(cmd, datas[i], datas_name[i + 1], para[i + 1]);
                            }
                            cmd.ExecuteNonQuery();

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        // ロールバック
                        transaction.Rollback();
                        MessageBox.Show(ex.ToString());
                        return false;
                    }
                }
                command.Close();
            }
            return true;
        }

        // 正常に終了したらtrueを返す
        public bool Data_Update(List<string> datas, string oid)
        {
            using (var command = new SqlConnection(con))
            {
                command.Open();
                using (SqlTransaction transaction = command.BeginTransaction())
                {
                    try
                    {
                        string s = "UPDATE dbo.todo SET Do = @do,Date = @date,Category = @category,Location = @location,Url = @url,Explanation = @explanation,Alarm = @alarm,Comp = @comp　WHERE Id = @oid";
                        // データベースにデータを追加していく
                        using (SqlCommand cmd = new SqlCommand(s, command, transaction))
                        {
                            // パラメータ追加(何も設定されていなければNULLを入れる)
                            for (int i = 0; i < datas.Count; i++)
                            {
                                Parameters_Add(cmd, datas[i], datas_name[i + 1], para[i + 1]);
                            }
                            cmd.Parameters.Add(new SqlParameter("@oid", oid));

                            cmd.ExecuteNonQuery();

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        // ロールバック
                        transaction.Rollback();
                        MessageBox.Show("変更できませんでした\n" + ex.ToString());
                        return false;
                    }
                }
                command.Close();
            }
            return true;
        }

        // 該当のデータの削除
        public void Delete(string s, string datas,string datas_name)
        {
            try
            {
                using (var command = new SqlConnection(con))

                // コマンドオブジェクトを作成します。
                using (var cmd = command.CreateCommand())
                {
                    // コネクションをオープンします。
                    command.Open();
                    // データ削除のSQLを実行します。
                    cmd.CommandText = s;
                    // パラメータ追加(何も設定されていなければNULLを入れる)
                    cmd.Parameters.Add(new SqlParameter(datas_name, datas));
                    
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // パラメータ追加(cmd、値、値の名前、型)
        // 型 … 0：NVarChar、1：Int、2：DateTime、3：NChar
        private void Parameters_Add(SqlCommand cmd, string str, string name, int n)
        {
            // NVarChar
            if (n == 0)
            {
                // NULLではない場合
                if (str != "")
                {
                    cmd.Parameters.Add(name, SqlDbType.NVarChar).Value = str;
                }
                else // NULLの場合
                {
                    cmd.Parameters.Add(name, SqlDbType.NVarChar).Value = SqlString.Null;
                }
            }
            else if (n == 1) // Int
            {
                int data = int.Parse(str);
                // NULLではない場合
                if (data >= 0)
                {
                    cmd.Parameters.Add(name, SqlDbType.Int).Value = data;
                }
                else // NULLの場合
                {
                    cmd.Parameters.Add(name, SqlDbType.Int).Value = SqlInt32.Null;
                }
            }
            else if (n == 2) // DateTime
            {
                DateTime data;
                if (str == "") // 設定しない時
                {
                    data = new DateTime(1000, 1, 1, 0, 0, 0);
                }
                else
                {
                    data = DateTime.Parse(str);
                }
                
                // 1000年に設定されているものはNULL扱い
                if (data.Year > 1000)
                {
                    cmd.Parameters.Add(name, SqlDbType.DateTime).Value = data;
                }
                else // NULLの場合
                {
                    cmd.Parameters.Add(name, SqlDbType.DateTime).Value = SqlDateTime.Null;
                }
            }
        }

    }
}
