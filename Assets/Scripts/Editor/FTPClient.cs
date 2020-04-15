using System;
using System.IO;
using System.Net;
using UnityEngine;


namespace EndlessGame.Editor
{
    public interface IFTPClient
    {
        void UploadFile(string pRemoteFile, string pLocalFile);
        void UploadData(string pRemoteFile, byte[] pData);
        void UploadDataAsync(string pRemoteFile, byte[] pData);
        void CreateDirectory(string pNewDirectory);
        string[] FetchDirectoryInfo(string pDirectory);
    }


    public class FTPClient : IFTPClient
    {
        private string m_host = null;
        private string m_user = null;
        private string m_pass = null;
        private FtpWebRequest m_ftpRequest = null;
        private FtpWebResponse m_ftpResponse = null;
        private Stream m_ftpStream = null;
        private int m_bufferSize = 2048;
            

        public FTPClient(string pHostIp, string pUserName, string pPassword)
        {
            m_host = pHostIp;
            m_user = pUserName;
            m_pass = pPassword;
        }

        public void UploadFile(string pRemoteFile, string pLocalFile)
        {
            try
            {
                m_ftpRequest = (FtpWebRequest)FtpWebRequest.Create(m_host + "/" + pRemoteFile);
                m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);

                m_ftpRequest.UseBinary = true;
                m_ftpRequest.UsePassive = true;
                m_ftpRequest.KeepAlive = true;
                m_ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                
                m_ftpStream = m_ftpRequest.GetRequestStream();
                FileStream _localFileStream = new FileStream(pLocalFile, FileMode.Open);
                
                byte[] _byteBuffer = new byte[m_bufferSize];
                int _bytesSent = _localFileStream.Read(_byteBuffer, 0, m_bufferSize);
                
                try
                {
                    while (_bytesSent != 0)
                    {
                        m_ftpStream.Write(_byteBuffer, 0, _bytesSent);
                        _bytesSent = _localFileStream.Read(_byteBuffer, 0, m_bufferSize);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("FTPClient [ERROR]: {0}.", e.ToString());
                }

                _localFileStream.Close();
                m_ftpStream.Close();
                m_ftpRequest = null;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("FTPClient [ERROR]: {0}.", e.ToString());
            }

            return;
        }

        public void UploadData(string pRemoteFile, byte[] pData)
        {
            try
            {
                m_ftpRequest = (FtpWebRequest)FtpWebRequest.Create(m_host + "/" + pRemoteFile);
                m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);

                m_ftpRequest.UseBinary = true;
                m_ftpRequest.UsePassive = true;
                m_ftpRequest.KeepAlive = true;
                m_ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                
                m_ftpStream = m_ftpRequest.GetRequestStream();

                try
                {
                    m_ftpStream.Write(pData, 0, pData.Length);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("FTPClient [ERROR]: {0}.", e.ToString());
                }

                m_ftpStream.Close();
                m_ftpRequest = null;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("FTPClient [ERROR]: {0}.", e.ToString());
            }

            return;
        }

        public async void UploadDataAsync(string pRemoteFile, byte[] pData)
        {
            try
            {
                m_ftpRequest = (FtpWebRequest)FtpWebRequest.Create(m_host + "/" + pRemoteFile);
                m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);

                m_ftpRequest.UseBinary = true;
                m_ftpRequest.UsePassive = true;
                m_ftpRequest.KeepAlive = true;
                m_ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                
                m_ftpStream = await m_ftpRequest.GetRequestStreamAsync();

                try
                {
                    await m_ftpStream.WriteAsync(pData, 0, pData.Length);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("FTPClient [ERROR]: {0}.", e.ToString());
                }

                m_ftpStream.Close();
                m_ftpRequest = null;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("FTPClient [ERROR]: {0}.", e.ToString());
            }

            return;
        }

        public void CreateDirectory(string pNewDirectory)
        {
            try
            {
                m_ftpRequest = (FtpWebRequest)WebRequest.Create(m_host + "/" + pNewDirectory);
                m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);
                
                m_ftpRequest.UseBinary = true;
                m_ftpRequest.UsePassive = true;
                m_ftpRequest.KeepAlive = true;
                m_ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                
                m_ftpResponse = (FtpWebResponse)m_ftpRequest.GetResponse();
                
                m_ftpResponse.Close();
                m_ftpRequest = null;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("FTPClient [ERROR]: {0}.", e.ToString());
            }

            return;
        }

        public string[] FetchDirectoryInfo(string pDirectory)
        {
            try
            {
                m_ftpRequest = (FtpWebRequest)FtpWebRequest.Create(m_host + "/" + pDirectory);
                m_ftpRequest.Credentials = new NetworkCredential(m_user, m_pass);
                
                m_ftpRequest.UseBinary = true;
                m_ftpRequest.UsePassive = true;
                m_ftpRequest.KeepAlive = true;
                m_ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                
                m_ftpResponse = (FtpWebResponse)m_ftpRequest.GetResponse();
                m_ftpStream = m_ftpResponse.GetResponseStream();
                
                StreamReader _ftpReader = new StreamReader(m_ftpStream);
                string _directoryRaw = null;
                
                try
                {
                    while (_ftpReader.Peek() != -1)
                    {
                        _directoryRaw += _ftpReader.ReadLine() + "|";
                    }
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("FTPClient [ERROR]: {0}.", e.ToString());
                }
                
                _ftpReader.Close();
                m_ftpStream.Close();
                m_ftpResponse.Close();
                m_ftpRequest = null;
                
                try
                {
                    string[] _directoryList = _directoryRaw.Split("|".ToCharArray());
                    return _directoryList;
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("FTPClient [ERROR]: {0}.", e.ToString());
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("FTPClient [ERROR]: {0}.", e.ToString());
            }
            
            return new string[] { string.Empty };
        }
    }
}