using System;
using Elders.VSE_FormatDocumentOnSave.Configurations;
using EnvDTE;

namespace Elders.VSE_FormatDocumentOnSave
{
    public class DocumentFormatService
    {
        private readonly DTE dte;
        readonly Func<Document, IConfiguration> getGeneralCfg;
        readonly IDocumentFormatter formatter;

        public DocumentFormatService(DTE dte, Func<Document, IConfiguration> getGeneralCfg)
        {
            this.dte = dte;
            this.getGeneralCfg = getGeneralCfg;

            formatter = new VisualStudioCommandFormatter(dte);
        }

        public void FormatDocument(Document doc)
        {
            if (ShouldFormat(doc) == false)
                return;

            try
            {
                var cfg = getGeneralCfg(doc);
                var filter = new AllowDenyDocumentFilter(cfg.Allowed, cfg.Denied, cfg.ExcludedPaths);

                foreach (string splitCommand in cfg.Commands.Trim().Split(' '))
                {
                    try
                    {
                        string commandName = splitCommand.Trim();
                        formatter.Format(doc, filter, commandName);
                    }
                    catch (Exception) { }   // may be we can log which command has failed and why
                }
            }
            catch (Exception) { }   // Do not do anything here on purpose.
        }

        private bool ShouldFormat(Document doc)
        {
            if (System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock))
                return false;

            bool vsIsInDebug = dte.Mode == vsIDEMode.vsIDEModeDebug;
            var cfg = getGeneralCfg(doc);

            if (vsIsInDebug == true && cfg.EnableInDebug == false)
                return false;

            return cfg.IsEnable;
        }
    }
}