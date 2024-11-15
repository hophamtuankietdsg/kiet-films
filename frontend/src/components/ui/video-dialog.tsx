import { Dialog, DialogContent, DialogHeader, DialogTitle } from './dialog';

interface VideoDialogProps {
  isOpen: boolean;
  onClose: () => void;
  videoKey?: string;
  title: string;
}

export default function VideoDialog({
  isOpen,
  onClose,
  videoKey,
  title,
}: VideoDialogProps) {
  if (!videoKey) return null;

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[1000px] max-h-[90vh]">
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
        </DialogHeader>
        <div className="aspect-video w-full">
          <iframe
            width="100%"
            height="100%"
            src={`https://www.youtube.com/embed/${videoKey}`}
            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
            allowFullScreen
            className="rounded-lg"
          />
        </div>
      </DialogContent>
    </Dialog>
  );
}
