using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace TrainStationServer
{
    class eXosip
    {
        private static IntPtr eXosipContext = IntPtr.Zero;

        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr eXosip_malloc();
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int eXosip_init(IntPtr context);
        public static void Init()
        {
            if (eXosipContext == IntPtr.Zero)
                eXosipContext = eXosip_malloc();
            int i = eXosip_init(eXosipContext);
            osip.ThrowException(i);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static void eXosip_quit(IntPtr context);
        public static void Quit() 
        {
            if (eXosipContext != IntPtr.Zero)
            {
                eXosip_quit(eXosipContext);
                eXosipContext = IntPtr.Zero;
            }
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int eXosip_set_option(IntPtr excontext, EXOSIP_OPT opt, IntPtr value);
        public enum EXOSIP_OPT
        {
            UDP_KEEP_ALIVE = 1
        }
        public static void SetOption(EXOSIP_OPT opt, IntPtr value)
        {
            int i = eXosip_set_option(eXosipContext, opt, value);
            osip.ThrowException(i);
        }
        
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int eXosip_lock(IntPtr context);
        public static void Lock() 
        {
            int i = eXosip_lock(eXosipContext);
            osip.ThrowException(i);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int eXosip_unlock(IntPtr context);
        public static void Unlock() 
        {
            int i = eXosip_unlock(eXosipContext);
            osip.ThrowException(i);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static void eXosip_automatic_action(IntPtr context);
        public static void AutomaticAction() 
        {
            eXosip_automatic_action(eXosipContext);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int eXosip_default_action(IntPtr context, IntPtr je);
        public static void DefaultAction(IntPtr je) 
        {
            int i = eXosip_default_action(eXosipContext, je);
            //osip.ThrowException(i);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int eXosip_guess_localip(IntPtr context, AddressFamily family, StringBuilder address, int size);
        public static string GuessLocalIP(AddressFamily family) 
        { 
            StringBuilder localIP = new StringBuilder(16);
            int i = eXosip_guess_localip(eXosipContext, family, localIP, localIP.Capacity);
            osip.ThrowException(i);
            return localIP.ToString();
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int eXosip_listen_addr(IntPtr context, ProtocolType transport, string addr, int port, AddressFamily family, int secure);
        public static void ListenAddr(ProtocolType transport, string addr, int port, AddressFamily family, int secure) 
        { 
            int i = eXosip_listen_addr(eXosipContext, transport,addr,port,family,secure);
            osip.ThrowException(i);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int eXosip_add_authentication_info(IntPtr context, string username, string userid, string passwd, string ha1, string realm);
        public static void AddAuthenticationInfo(string username, string userid, string passwd, string ha1, string realm)
        { 
            int i = eXosip_add_authentication_info(eXosipContext, username, userid, passwd, ha1, realm);
            osip.ThrowException(i);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int eXosip_clear_authentication_info(IntPtr context);
        public static void ClearAuthenticationInfo()
        {
            int i = eXosip_clear_authentication_info(eXosipContext);
            osip.ThrowException(i);      
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr eXosip_get_local_sdp(IntPtr context, int did);
        public static IntPtr GetLocalSdp(int did)
        {
            return eXosip_get_local_sdp(eXosipContext, did);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr eXosip_get_remote_sdp(IntPtr context, int did);
        public static IntPtr GetRemoteSdp(int did)
        {
            return eXosip_get_remote_sdp(eXosipContext, did);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr eXosip_get_remote_sdp_from_tid(IntPtr context, int tid);
        public static IntPtr GetRemoteSdpFromTid(int tid)
        {
            return eXosip_get_remote_sdp_from_tid(eXosipContext, tid);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr eXosip_get_video_connection(IntPtr sdp);
        public static IntPtr GetVideoConnection(IntPtr sdp)
        {
            return eXosip_get_video_connection(sdp);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr eXosip_get_video_media(IntPtr sdp);
        public static IntPtr GetVideoMedia(IntPtr sdp)
        {
            return eXosip_get_video_media(sdp);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr eXosip_get_audio_connection(IntPtr sdp);
        public static IntPtr GetAudioConnection(IntPtr sdp)
        {
            return eXosip_get_audio_connection(sdp);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr eXosip_get_audio_media(IntPtr sdp);
        public static IntPtr GetAudioMedia(IntPtr sdp)
        {
            return eXosip_get_audio_media(sdp);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr eXosip_get_media(IntPtr sdp, string media);
        public static IntPtr GetMedia(IntPtr sdp, string media)
        {
            return eXosip_get_media(sdp, media);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr eXosip_get_connection(IntPtr sdp, string media);
        public static IntPtr GetConnection(IntPtr sdp, string media)
        {
            return eXosip_get_connection(sdp, media);
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int eXosip_build_publish(IntPtr context, out IntPtr message, string to, string from, string route, string _event, string expires, string ctype, string body);
        public static IntPtr BuildPublish(string to, string from, string route, string _event, string expires, string ctype, string body)
        {
            IntPtr pub;
            int i = eXosip_build_publish(eXosipContext, out pub, to, from, route, _event, expires, ctype, body);
            osip.ThrowException(i);
            return pub;
        }
        [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int eXosip_publish(IntPtr context, IntPtr message, string to);
        public static void Publish(IntPtr message, string to)
        {
            int i = eXosip_publish(eXosipContext, message, to);
            osip.ThrowException(i);
        }
        public enum EventType
        {
            /* REGISTER related events */
            EXOSIP_REGISTRATION_NEW,         /**< announce new registration.       */
            EXOSIP_REGISTRATION_SUCCESS,     /**< user is successfully registred.  */
            EXOSIP_REGISTRATION_FAILURE,     /**< user is not registred.           */
            EXOSIP_REGISTRATION_REFRESHED,   /**< registration has been refreshed. */
            EXOSIP_REGISTRATION_TERMINATED,  /**< UA is not registred any more.    */

            /* INVITE related events within calls */
            EXOSIP_CALL_INVITE,          /**< announce a new call                   */
            EXOSIP_CALL_REINVITE,        /**< announce a new INVITE within call     */

            EXOSIP_CALL_NOANSWER,        /**< announce no answer within the timeout */
            EXOSIP_CALL_PROCEEDING,      /**< announce processing by a remote app   */
            EXOSIP_CALL_RINGING,         /**< announce ringback                     */
            EXOSIP_CALL_ANSWERED,        /**< announce start of call                */
            EXOSIP_CALL_REDIRECTED,      /**< announce a redirection                */
            EXOSIP_CALL_REQUESTFAILURE,  /**< announce a request failure            */
            EXOSIP_CALL_SERVERFAILURE,   /**< announce a server failure             */
            EXOSIP_CALL_GLOBALFAILURE,   /**< announce a global failure             */
            EXOSIP_CALL_ACK,             /**< ACK received for 200ok to INVITE      */

            EXOSIP_CALL_CANCELLED,       /**< announce that call has been cancelled */
            EXOSIP_CALL_TIMEOUT,         /**< announce that call has failed         */

            /* request related events within calls (except INVITE) */
            EXOSIP_CALL_MESSAGE_NEW,            /**< announce new incoming request. */
            EXOSIP_CALL_MESSAGE_PROCEEDING,     /**< announce a 1xx for request. */
            EXOSIP_CALL_MESSAGE_ANSWERED,       /**< announce a 200ok  */
            EXOSIP_CALL_MESSAGE_REDIRECTED,     /**< announce a failure. */
            EXOSIP_CALL_MESSAGE_REQUESTFAILURE, /**< announce a failure. */
            EXOSIP_CALL_MESSAGE_SERVERFAILURE,  /**< announce a failure. */
            EXOSIP_CALL_MESSAGE_GLOBALFAILURE,  /**< announce a failure. */

            EXOSIP_CALL_CLOSED,          /**< a BYE was received for this call      */

            /* for both UAS & UAC events */
            EXOSIP_CALL_RELEASED,           /**< call context is cleared.            */

            /* response received for request outside calls */
            EXOSIP_MESSAGE_NEW,            /**< announce new incoming request. */
            EXOSIP_MESSAGE_PROCEEDING,     /**< announce a 1xx for request. */
            EXOSIP_MESSAGE_ANSWERED,       /**< announce a 200ok  */
            EXOSIP_MESSAGE_REDIRECTED,     /**< announce a failure. */
            EXOSIP_MESSAGE_REQUESTFAILURE, /**< announce a failure. */
            EXOSIP_MESSAGE_SERVERFAILURE,  /**< announce a failure. */
            EXOSIP_MESSAGE_GLOBALFAILURE,  /**< announce a failure. */

            /* Presence and Instant Messaging */
            EXOSIP_SUBSCRIPTION_UPDATE,       /**< announce incoming SUBSCRIBE.      */
            EXOSIP_SUBSCRIPTION_CLOSED,       /**< announce end of subscription.     */

            EXOSIP_SUBSCRIPTION_NOANSWER,        /**< announce no answer              */
            EXOSIP_SUBSCRIPTION_PROCEEDING,      /**< announce a 1xx                  */
            EXOSIP_SUBSCRIPTION_ANSWERED,        /**< announce a 200ok                */
            EXOSIP_SUBSCRIPTION_REDIRECTED,      /**< announce a redirection          */
            EXOSIP_SUBSCRIPTION_REQUESTFAILURE,  /**< announce a request failure      */
            EXOSIP_SUBSCRIPTION_SERVERFAILURE,   /**< announce a server failure       */
            EXOSIP_SUBSCRIPTION_GLOBALFAILURE,   /**< announce a global failure       */
            EXOSIP_SUBSCRIPTION_NOTIFY,          /**< announce new NOTIFY request     */

            EXOSIP_SUBSCRIPTION_RELEASED,        /**< call context is cleared.        */

            EXOSIP_IN_SUBSCRIPTION_NEW,          /**< announce new incoming SUBSCRIBE.*/
            EXOSIP_IN_SUBSCRIPTION_RELEASED,     /**< announce end of subscription.   */

            EXOSIP_NOTIFICATION_NOANSWER,        /**< announce no answer              */
            EXOSIP_NOTIFICATION_PROCEEDING,      /**< announce a 1xx                  */
            EXOSIP_NOTIFICATION_ANSWERED,        /**< announce a 200ok                */
            EXOSIP_NOTIFICATION_REDIRECTED,      /**< announce a redirection          */
            EXOSIP_NOTIFICATION_REQUESTFAILURE,  /**< announce a request failure      */
            EXOSIP_NOTIFICATION_SERVERFAILURE,   /**< announce a server failure       */
            EXOSIP_NOTIFICATION_GLOBALFAILURE,   /**< announce a global failure       */

            EXOSIP_EVENT_COUNT                /**< MAX number of events              */
        };
        public struct Event
        {
            public EventType type;               /**< type of the event */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string textinfo;      /**< text description of event */
            public IntPtr external_reference; /**< external reference (for calls) */

            public IntPtr request;   /**< request within current transaction */
            public IntPtr response;  /**< last response within current transaction */
            public IntPtr ack;       /**< ack within current transaction */

            public int tid; /**< unique id for transactions (to be used for answers) */
            public int did; /**< unique id for SIP dialogs */

            public int rid; /**< unique id for registration */
            public int cid; /**< unique id for SIP calls (but multiple dialogs!) */
            public int sid; /**< unique id for outgoing subscriptions */
            public int nid; /**< unique id for incoming subscriptions */

            public int ss_status;  /**< current Subscription-State for subscription */
            public int ss_reason;  /**< current Reason status for subscription */

            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static IntPtr eXosip_event_wait(IntPtr context, int tv_s, int tv_ms);
            public static IntPtr Wait(int tv_s, int tv_ms)
            {
                return eXosip_event_wait(eXosipContext, tv_s, tv_ms);
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static IntPtr eXosip_event_get(IntPtr context);
            public static IntPtr Get()
            {
                return eXosip_event_get(eXosipContext);
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static void eXosip_event_free(IntPtr je);
            public static void Free(IntPtr je)
            {
                eXosip_event_free(je);
            }
        };

        public static class Register
        {
            private static int _rid;
            private static int _expires;
            private static string _from;
            private static string _proxy;
            private static string _contact;
            private static IntPtr reg;

            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_register_build_initial_register(IntPtr context, string from, string proxy, string contact, int expires, out IntPtr reg);
            public static int BuildInitialRegister(string from, string proxy, string contact, int expires, out IntPtr reg)
            {
                int i = eXosip_register_build_initial_register(eXosipContext, from, proxy, contact, expires, out reg);
                //osip.ThrowException(i);
                return i;
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_register_build_register(IntPtr context, int rid, int expires, out IntPtr reg);
            public static IntPtr BuildRegister(int rid, int expires)
            {
                IntPtr reg;
                int i = eXosip_register_build_register(eXosipContext, rid, expires, out reg);
                //osip.ThrowException(i);
                return reg;
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_register_send_register(IntPtr context, int rid, IntPtr reg);
            public static void SendRegister(int rid, IntPtr reg)
            {
                int i = eXosip_register_send_register(eXosipContext, rid, reg);
                //osip.ThrowException(i);
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_register_remove(IntPtr context, int rid);
            public static void Initialize(string from, string proxy, string contact, int expires)
            {
                _from = from;
                _proxy = proxy;
                _contact = contact;
                _expires = expires;
                if (_rid > 0)
                    eXosip_register_remove(eXosipContext, _rid);
                _rid = eXosip_register_build_initial_register(eXosipContext, _from, _proxy, _contact, _expires, out reg);
            }
            public static void Send()
            {
                int i;
                if (_rid <= 0)
                {
                    i = eXosip_register_build_initial_register(eXosipContext, _from, _proxy, _contact, _expires, out reg);
                    _rid = i;
                }
                else
                {
                    i = eXosip_register_build_register(eXosipContext, _rid, _expires, out reg);
                }
                if (i >= 0)
                {
                    eXosip_register_send_register(eXosipContext, _rid, reg);
                }
                else
                {
                    eXosip_register_remove(eXosipContext,_rid);
                }
            }
        }
        public static class Option
        { 
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_options_build_request(IntPtr context, out IntPtr options, string to,string from, string route);
            public static IntPtr BuildRequest(string to, string from, string route)
            {
                IntPtr opt;
                eXosip_options_build_request(eXosipContext, out opt, to, from, route);
                return opt;
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_options_send_request(IntPtr context, IntPtr options);
            public static void SendRequest(IntPtr opt)
            {
                eXosip_options_send_request(eXosipContext, opt);
            }
        }
        public static class Call
        {
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_call_build_initial_invite(IntPtr context, out IntPtr invite, string to, string from, string route, string subject);
            public static IntPtr BuildInitialInvite(string to, string from, string route, string subject)
            {
                IntPtr invite;
                int i = eXosip_call_build_initial_invite(eXosipContext, out invite, to, from, route, subject);
                osip.ThrowException(i);
                return invite;
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_call_send_initial_invite(IntPtr context, IntPtr invite);
            public static int SendInitialInvite(IntPtr invite)
            {
                int i = eXosip_call_send_initial_invite(eXosipContext, invite);
                osip.ThrowException(i);
                return i;
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_call_set_reference(IntPtr context, int id, IntPtr reference);
            public static void SetReference(int id, IntPtr reference)
            {
                int i = eXosip_call_set_reference(eXosipContext, id, reference);
                osip.ThrowException(i);
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_call_build_answer(IntPtr context, int tid, int status, out IntPtr answer);
            public static IntPtr BuildAnswer(int tid, int status)
            {
                IntPtr answer;
                int i = eXosip_call_build_answer(eXosipContext, tid, status, out answer);
                osip.ThrowException(i);
                return answer;
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_call_send_answer(IntPtr context, int tid, int status, IntPtr answer);
            public static void SendAnswer(int tid, int status, IntPtr answer)
            {
                int i = eXosip_call_send_answer(eXosipContext, tid, status, answer);
                osip.ThrowException(i);
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_call_build_ack(IntPtr context, int did, out IntPtr ack);
            public static IntPtr BuildAck(int did)
            {
                IntPtr ack;
                int i = eXosip_call_build_ack(eXosipContext, did, out ack);
                //osip.ThrowException(i);
                return ack;
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_call_build_info(IntPtr context, int did, out IntPtr request);
            public static IntPtr BuildInfo(int did)
            {
                IntPtr info;
                int i = eXosip_call_build_info(eXosipContext, did, out info);
                //osip.ThrowException(i);
                return info;
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_call_build_options(IntPtr context, int did, out IntPtr request);
            public static IntPtr BuildOptions(int did)
            {
                IntPtr opt;
                int i = eXosip_call_build_options(eXosipContext, did, out opt);
                osip.ThrowException(i);
                return opt;
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_call_build_update(IntPtr context, int did, out IntPtr request);
            public static IntPtr BuildUpdate(int did)
            {
                IntPtr update;
                int i = eXosip_call_build_update(eXosipContext, did, out update);
                osip.ThrowException(i);
                return update;
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_call_send_ack(IntPtr context, int did, IntPtr ack);
            public static void SendAck(int did, IntPtr ack)
            {
                int i = eXosip_call_send_ack(eXosipContext, did, ack);
                osip.ThrowException(i);
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_call_send_request(IntPtr context, int did, IntPtr request);
            public static void SendRequest(int did, IntPtr request)
            {
                int i = eXosip_call_send_request(eXosipContext, did, request);
                osip.ThrowException(i);
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_call_terminate(IntPtr context, int cid, int did);
            public static void Terminate(int cid, int did)
            {
                eXosip_call_terminate(eXosipContext, cid, did);
            }
        }
        public static class Message
        {
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_message_build_request(IntPtr context, out IntPtr message, string method, string to, string from, string route);
            public static IntPtr BuildRequest(string method, string to, string from, string route)
            {
                IntPtr msg;
                int i = eXosip_message_build_request(eXosipContext, out msg, method, to, from, route);
                osip.ThrowException(i);
                return msg;
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_message_send_request(IntPtr context, IntPtr message);
            public static void SendRequest(IntPtr message)
            {
                int i = eXosip_message_send_request(eXosipContext, message);
                osip.ThrowException(i);
            }
        }
        public static class Subscribe
        {
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_subscribe_build_initial_request(IntPtr context, out IntPtr subscribe, string to, string from, string route, string evt, int expires);
            public static IntPtr BuildInitialRequest(string to, string from, string route, string evt, int expires)
            {
                Console.WriteLine(route);
                IntPtr subscribe;
                int i = eXosip_subscribe_build_initial_request(eXosipContext, out subscribe, to, from, route, evt, expires);
                osip.ThrowException(i);
                return subscribe;
            }
            [DllImport("eXosip.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int eXosip_subscribe_send_initial_request(IntPtr context, IntPtr subscribe);
            public static void SendInitialRequest(IntPtr subscribe)
            {
                int i = eXosip_subscribe_send_initial_request(eXosipContext, subscribe);
                osip.ThrowException(i);
            }
        }
    }

    class osip
    {
        enum Error
        {
            OSIP_SUCCESS = 0,
            OSIP_UNDEFINED_ERROR = -1,
            OSIP_BADPARAMETER = -2,
            OSIP_WRONG_STATE = -3,
            OSIP_NOMEM = -4,
            OSIP_SYNTAXERROR = -5,
            OSIP_NOTFOUND = -6,
            OSIP_API_NOT_INITIALIZED = -7,
            OSIP_NO_NETWORK = -10,
            OSIP_PORT_BUSY = -11,
            OSIP_UNKNOWN_HOST = -12,
            OSIP_DISK_FULL = -30,
            OSIP_NO_RIGHTS = -31,
            OSIP_FILE_NOT_EXIST = -32,
            OSIP_TIMEOUT = -50,
            OSIP_TOOMUCHCALL = -51,
            OSIP_WRONG_FORMAT = -52,
            OSIP_NOCOMMONCODEC = -53
        }

        public static void ThrowException(int code)
        {
            if (code < 0)
            {
                throw new Exception(((Error)code).ToString());
            }
        }

        public class Message
        {
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int osip_message_set_contact(IntPtr sip, string hvalue);
            public static void SetContact(IntPtr sip, string hvalue)
            {
                int i = osip_message_set_contact(sip, hvalue);
                if (i < 0) throw new Exception(((Error)i).ToString());
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int osip_message_get_contact(IntPtr sip, int pos, out IntPtr dest);
            public static IntPtr GetContact(IntPtr sip)
            {
                IntPtr ptr;
                int i = osip_message_get_contact(sip, 0, out ptr);
                if (i < 0) throw new Exception(((Error)i).ToString());
                return ptr;         
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int osip_message_get_body(IntPtr sip, int pos, out IntPtr dest);
            public static IntPtr GetBody(IntPtr sip)
            {
                IntPtr ptr;
                osip_message_get_body(sip, 0, out ptr);
                return ptr;
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int osip_message_set_body(IntPtr sip, string buf, uint length);
            public static void SetBody(IntPtr sip, string buf)
            {
                int i = osip_message_set_body(sip, buf, (uint)Encoding.UTF8.GetByteCount(buf));
                if (i < 0) throw new Exception(((Error)i).ToString());
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int osip_message_set_content_type(IntPtr sip, string hvalue);
            public static void SetContentType(IntPtr sip, string hvalue)
            {
                int i = osip_message_set_content_type(sip, hvalue);
                if (i < 0) throw new Exception(((Error)i).ToString());
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static IntPtr osip_message_get_content_type(IntPtr sip);
            public static IntPtr GetContentType(IntPtr sip)
            {
                return osip_message_get_content_type(sip);
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int osip_message_to_str(IntPtr sip, out IntPtr dest, out uint message_length);
            public static string ToString(IntPtr sip)
            {
                IntPtr ptr;
                uint length;
                int i = osip_message_to_str(sip, out ptr, out length);
                if (i < 0) throw new Exception(((Error)i).ToString());
                string str = Marshal.PtrToStringAnsi(ptr);
                osip_free(ptr);
                return str;
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int osip_message_set_header(IntPtr sip, string hname, string hvalue);
            public static void SetHeader(IntPtr sip, string hname, string hvalue)
            {
                int i = osip_message_set_header(sip, hname, hvalue);
                if (i < 0) throw new Exception(((Error)i).ToString());
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int osip_message_get_status_code(IntPtr sip);
            public static int GetStatusCode(IntPtr sip)
            {
                int i = osip_message_get_status_code(sip);
                if (i < 0) throw new Exception(((Error)i).ToString());
                return i;
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static IntPtr osip_message_get_method(IntPtr sip);
            public static string GetMethod(IntPtr sip)
            {
                return Marshal.PtrToStringAnsi(osip_message_get_method(sip));
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static IntPtr osip_message_get_uri(IntPtr sip);
            public static URI GetURI(IntPtr sip)
            {
                IntPtr ptr = osip_message_get_uri(sip);
                return (URI)Marshal.PtrToStructure(ptr, typeof(URI));
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static IntPtr osip_message_get_to(IntPtr sip);
            public static From GetTo(IntPtr sip)
            {
                IntPtr ptr = osip_message_get_to(sip);
                return (From)Marshal.PtrToStructure(ptr, typeof(From));
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static IntPtr osip_message_get_from(IntPtr sip);
            public static From GetFrom(IntPtr sip)
            {
                IntPtr ptr = osip_message_get_from(sip);
                return (From)Marshal.PtrToStructure(ptr, typeof(From));
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int osip_message_header_get_byname(IntPtr sip, string hname, int pos, out IntPtr dest);
            public static IntPtr GetHeaderByName(IntPtr sip, string hname, int pos)
            {
                IntPtr header;
                int i = osip_message_header_get_byname(sip, hname, pos, out header);
                //if (i < 0) throw new Exception(((Error)i).ToString());
                return header;
            }
        }
        public static class SdpMessage
        {
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static void sdp_message_free(IntPtr sdp);
            public static void Free(IntPtr sdp)
            {
                sdp_message_free(sdp);
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int sdp_message_to_str(IntPtr sdp, out IntPtr dest);
            public static IntPtr ToString(IntPtr sdp)
            {
                IntPtr str;
                sdp_message_to_str(sdp, out str);
                return str;
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static IntPtr sdp_message_s_name_get(IntPtr sdp);
            public static string GetSessionName(IntPtr sdp)
            {
                IntPtr ptr = sdp_message_s_name_get(sdp);
                string str = null;
                if (ptr != IntPtr.Zero)
                {
                    str =  Marshal.PtrToStringAnsi(ptr);
                    osip_free(ptr);
                }
                return str;
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static string sdp_message_o_sess_id_get(IntPtr sdp);
            public static string GetSessionId(IntPtr sdp)
            {
                return sdp_message_o_sess_id_get(sdp);
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static string sdp_message_o_sess_version_get(IntPtr sdp);
            public static string GetSessionVersion(IntPtr sdp)
            {
                return sdp_message_o_sess_version_get(sdp);
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static string sdp_message_t_start_time_get(IntPtr sdp, int pos_td);
            public static string GetStartTime(IntPtr sdp)
            {
                return sdp_message_t_start_time_get(sdp, 0);
            }
        }
        [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static void osip_free(IntPtr ptr);
        public static void Free(IntPtr ptr)
        {
            osip_free(ptr);
        }
        [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static void osip_header_free(IntPtr header);
        public static void FreeHeader(IntPtr header)
        {
            osip_header_free(header);
        }

        //public static void SendMessage(string to, string body, string type)
        //{
        //    string from = string.Format("sip:{0}@ivms.com", Program.User);
        //    string route = string.Format("<sip:{0};lr>", Program.SvrIP);
        //    IntPtr message = eXosip.Message.BuildRequest("MESSAGE", to, from, route);
        //    Message.SetBody(message, body);
        //    Message.SetContentType(message, type);
        //    eXosip.Message.SendRequest(message);
        //}

        public struct Header
        {
            public IntPtr hname;     /**< Name of header */
            public IntPtr hvalue;    /**< Value for header */
        };

        public struct From
        {
            public IntPtr displayname;       /**< Display Name */
            public IntPtr url;         /**< url */
            public List gen_params;  /**< other From parameters */

            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int osip_from_to_str(IntPtr header, out IntPtr dest);
            public static string ToString(IntPtr header)
            {
                IntPtr ptr = IntPtr.Zero;
                string str = null;
                osip_from_to_str(header, out ptr);
                if (ptr != null)
                {
                    str = Marshal.PtrToStringAnsi(ptr);
                    osip_free(ptr);
                }
                return str;
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static IntPtr osip_from_get_url(IntPtr header);
            public static IntPtr GetURL(IntPtr header)
            {
                return osip_from_get_url(header);
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static void osip_from_free(IntPtr header);
            public static void Free(IntPtr header)
            {
                osip_from_free(header);
            }
            public static URI GetURL(From from)
            {
                return (URI)Marshal.PtrToStructure(from.url, typeof(URI));
            }
        };

        public struct URI
        {
            public IntPtr scheme;              /**< Uri Scheme (sip or sips) */
            public IntPtr username;            /**< Username */
            public IntPtr password;            /**< Password */
            public IntPtr host;                /**< Domain */
            public IntPtr port;                /**< Port number */
            public List url_params;    /**< Uri parameters */
            public List url_headers;   /**< Uri headers */
            public IntPtr str;  /**< Space for other url schemes. (http, mailto...) */

            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static void osip_uri_free(IntPtr uri);
            public static void Free(IntPtr uri)
            {
                osip_uri_free(uri);
            }
            [DllImport("osipparser2.dll", CallingConvention = CallingConvention.Cdecl)]
            private extern static int osip_uri_to_str(IntPtr url, out IntPtr dest);
            public static string ToString(IntPtr url)
            {
                IntPtr ptr = IntPtr.Zero;
                string str = null;
                osip_uri_to_str(url, out ptr);
                if (ptr != IntPtr.Zero)
                {
                    str = Marshal.PtrToStringAnsi(ptr);
                    osip_free(ptr);
                }
                return str;
            }
        };


        public struct List
        {
            public int nb_elt;         /**< Number of element in the list */
            public IntPtr node;     /**< Next node containing element  */

        };

        public struct node
        {
            public IntPtr next;                 /**< next __node_t containing element */
            public IntPtr element;              /**< element in Current node */
        };
        public struct Body
        {
            public IntPtr body;                        /**< buffer containing data */
            public uint length;                     /**< length of data */
            public IntPtr headers;              /**< List of headers (when mime is used) */
            public IntPtr content_type; /**< Content-Type (when mime is used) */
        };

        public struct ContentType
        {
            public IntPtr type;                 /**< Type of attachement */
            public IntPtr subtype;              /**< Sub-Type of attachement */
            public List gen_params;     /**< Content-Type parameters */
        };
        public static class SDP
        {
            public struct Connection
            {
                public IntPtr c_nettype;             /**< Network Type */
                public IntPtr c_addrtype;            /**< Network Address Type */
                public IntPtr c_addr;                /**< Address */
                public IntPtr c_addr_multicast_ttl;  /**< TTL value for multicast address */
                public IntPtr c_addr_multicast_int;  /**< Number of multicast address */
            };

            public struct Media
            {
                public IntPtr m_media;              /**< media type */
                public IntPtr m_port;               /**< port number */
                public IntPtr m_number_of_port;     /**< number of port */
                public IntPtr m_proto;              /**< protocol to be used */
                public List m_payloads;    /**< list of payloads (as strings) */

                public IntPtr i_info;               /**< information header */
                public List c_connections; /**< list of sdp_connection_t * */
                public List b_bandwidths;  /**< list of sdp_bandwidth_t * */
                public List a_attributes;  /**< list of sdp_attribute_t * */
                public IntPtr k_key;           /**< key informations */
            };

            public struct Key
            {
                public IntPtr k_keytype;    /**< Key Type (prompt, clear, base64, uri) */
                public IntPtr k_keydata;    /**< key data */
            };
        }
    }
}
